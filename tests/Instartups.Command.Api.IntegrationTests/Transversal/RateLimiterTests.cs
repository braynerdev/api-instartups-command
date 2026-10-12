using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Api.IntegrationTests.Transversal;

/// <summary>
/// Usa um host próprio com o rate limiter real: 5 tokens por usuário a cada 20s e fila de 2.
/// </summary>
[Collection(IntegrationCollection.Nome)]
public class RateLimiterTests(InstartupsApiComRateLimiterFactory factory) : IClassFixture<InstartupsApiComRateLimiterFactory>
{
    private const int Rajada = 8;

    private CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task RajadaDoMesmoUsuario_AcimaDoLimiteEDaFila_DeveRetornar429()
    {
        var cliente = CriarCliente(Guid.NewGuid());

        var rejeitada = await DispararRajadaAteRejeicaoAsync(cliente);

        rejeitada.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);
        var json = JsonDocument.Parse(await rejeitada.Content.ReadAsStringAsync(Ct)).RootElement;
        json.GetProperty("isSuccess").GetBoolean().ShouldBeFalse();
        json.GetProperty("message").GetString().ShouldBe(MensagensErroConst.LimiteRequisicoes);
    }

    [Fact]
    public async Task RajadaDeUmUsuario_NaoDeveLimitarOutroUsuario()
    {
        await DispararRajadaAteRejeicaoAsync(CriarCliente(Guid.NewGuid()));

        var resposta = await CriarCliente(Guid.NewGuid()).PatchAsync(Rotas.Perfil.Desativar, null, Ct);

        resposta.StatusCode.ShouldNotBe(HttpStatusCode.TooManyRequests);
        resposta.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private HttpClient CriarCliente(Guid usuarioId)
    {
        var cliente = factory.CreateClient();
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", factory.Tokens.Gerar(usuarioId));
        return cliente;
    }

    /// <summary>
    /// Dispara a rajada em paralelo e devolve a primeira resposta 429. As requisições que ficaram
    /// na fila do limiter são canceladas para o teste não esperar a reposição de 20s.
    /// </summary>
    private async Task<HttpResponseMessage> DispararRajadaAteRejeicaoAsync(HttpClient cliente)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        cts.CancelAfter(TimeSpan.FromSeconds(10));

        var pendentes = Enumerable.Range(0, Rajada)
            .Select(_ => cliente.PatchAsync(Rotas.Perfil.Desativar, null, cts.Token))
            .ToList();

        try
        {
            while (pendentes.Count > 0)
            {
                var concluida = await Task.WhenAny(pendentes);
                pendentes.Remove(concluida);

                if (concluida.IsCompletedSuccessfully && concluida.Result.StatusCode == HttpStatusCode.TooManyRequests)
                    return concluida.Result;
            }
        }
        finally
        {
            await cts.CancelAsync();
            await Task.WhenAll(pendentes.Select(t => t.ContinueWith(_ => { }, TaskScheduler.Default)));
        }

        throw new ShouldAssertException($"Nenhuma das {Rajada} requisições foi rejeitada com 429.");
    }
}
