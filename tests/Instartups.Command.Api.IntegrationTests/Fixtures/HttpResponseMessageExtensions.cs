using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Instartups.Command.Api.IntegrationTests.Fixtures;

public static class HttpResponseMessageExtensions
{
    private static readonly JsonSerializerOptions Opcoes = new(JsonSerializerDefaults.Web);

    public static async Task<RespostaApi<T>> LerCorpoAsync<T>(this HttpResponseMessage resposta)
    {
        var corpo = await resposta.Content.ReadFromJsonAsync<RespostaApi<T>>(Opcoes, TestContext.Current.CancellationToken);
        return corpo.ShouldNotBeNull();
    }

    public static async Task<JsonElement> LerJsonAsync(this HttpResponseMessage resposta)
    {
        var json = await resposta.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        return JsonDocument.Parse(json).RootElement.Clone();
    }

    public static async Task<RespostaApi<string>> DeveSerErroAsync(this HttpResponseMessage resposta, HttpStatusCode status, string mensagem)
    {
        resposta.StatusCode.ShouldBe(status, await resposta.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        var corpo = await resposta.LerCorpoAsync<string>();
        corpo.IsSuccess.ShouldBeFalse();
        corpo.Message.ShouldBe(mensagem);
        return corpo;
    }

    public static async Task DeveSerErroDeValidacaoAsync(this HttpResponseMessage resposta, string campo, string codigo)
    {
        resposta.StatusCode.ShouldBe(HttpStatusCode.BadRequest, await resposta.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        var corpo = await resposta.LerCorpoAsync<List<ErroValidacao>>();
        corpo.IsSuccess.ShouldBeFalse();
        corpo.Message.ShouldBe("Erro de validação.");
        corpo.Data.ShouldNotBeNull().ShouldContain(e => e.Campo == campo && e.Codigo == codigo,
            $"Esperado erro {codigo} em '{campo}'. Recebido: {string.Join(", ", corpo.Data!.Select(e => $"{e.Campo}:{e.Codigo}"))}");
    }

    public static async Task DeveSerNoContentAsync(this HttpResponseMessage resposta)
    {
        resposta.StatusCode.ShouldBe(HttpStatusCode.NoContent, await resposta.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    public static async Task<Guid> DeveSerCriadoAsync(this HttpResponseMessage resposta, string mensagem)
    {
        resposta.StatusCode.ShouldBe(HttpStatusCode.Created, await resposta.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        var corpo = await resposta.LerCorpoAsync<IdResposta>();
        corpo.IsSuccess.ShouldBeTrue();
        corpo.Message.ShouldBe(mensagem);
        var id = corpo.Data.ShouldNotBeNull().Id;
        id.ShouldNotBe(Guid.Empty);
        return id;
    }
}
