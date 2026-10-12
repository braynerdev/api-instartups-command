using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Instartups.Command.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Instartups.Command.Api.IntegrationTests.Fixtures;

[Collection(IntegrationCollection.Nome)]
public abstract class IntegrationTestBase(InstartupsApiFactory factory) : IAsyncLifetime
{
    protected InstartupsApiFactory Factory { get; } = factory;
    protected CancellationToken Ct => TestContext.Current.CancellationToken;

    public virtual async ValueTask InitializeAsync() => await Factory.ResetarBancoAsync();

    public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;

    protected HttpClient CriarCliente(Guid? usuarioId = null)
    {
        var cliente = Factory.CreateClient();

        if (usuarioId is not null)
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Factory.Tokens.Gerar(usuarioId.Value));

        return cliente;
    }

    protected HttpClient CriarClienteComToken(string token)
    {
        var cliente = Factory.CreateClient();
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return cliente;
    }

    protected Task<HttpResponseMessage> PostAsync<T>(HttpClient cliente, string rota, T corpo) =>
        cliente.PostAsJsonAsync(rota, corpo, Ct);

    protected Task<HttpResponseMessage> PutAsync<T>(HttpClient cliente, string rota, T corpo) =>
        cliente.PutAsJsonAsync(rota, corpo, Ct);

    protected Task<HttpResponseMessage> PatchAsync(HttpClient cliente, string rota) =>
        cliente.PatchAsync(rota, null, Ct);

    protected Task<HttpResponseMessage> PatchAsync<T>(HttpClient cliente, string rota, T corpo) =>
        cliente.PatchAsJsonAsync(rota, corpo, Ct);

    protected Task<HttpResponseMessage> PostAsync(HttpClient cliente, string rota) =>
        cliente.PostAsync(rota, null, Ct);

    protected Task<HttpResponseMessage> DeleteAsync(HttpClient cliente, string rota) =>
        cliente.DeleteAsync(rota, Ct);

    protected Task<HttpResponseMessage> EnviarJsonCruAsync(HttpClient cliente, HttpMethod metodo, string rota, string json) =>
        cliente.SendAsync(new HttpRequestMessage(metodo, rota)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        }, Ct);

    /// <summary>
    /// Persiste o grafo inteiro num único contexto: entidades referenciadas por navegação
    /// (autor da postagem, perfis da curtida etc.) entram junto, sem duplicar.
    /// </summary>
    protected async Task SemearAsync(params object[] entidades)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.AddRange(entidades);
        await db.SaveChangesAsync(Ct);
    }

    protected async Task<T> ConsultarAsync<T>(Func<AppDbContext, Task<T>> consulta)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        return await consulta(db);
    }
}
