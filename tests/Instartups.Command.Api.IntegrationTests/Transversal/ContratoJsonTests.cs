using System.Net;
using System.Text.Json;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Api.UnitTests.Builders;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Api.IntegrationTests.Transversal;

/// <summary>
/// Todas as respostas da API usam camelCase, inclusive as geradas fora dos controllers
/// (autenticação, middleware de exceções e model binding).
/// </summary>
public class ContratoJsonTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private static readonly string[] ChavesResposta = ["isSuccess", "message", "dateTimeResponse", "data"];
    private static readonly string[] ChavesErroValidacao = ["campo", "codigo", "mensagem"];

    private readonly Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public async Task Resposta201_DeveUsarCamelCase()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Base, DtoBuilder.CadastrarPostagem());

        resposta.StatusCode.ShouldBe(HttpStatusCode.Created);
        var json = await resposta.LerJsonAsync();
        DeveTerExatamenteAsChaves(json, ChavesResposta);
        DeveTerExatamenteAsChaves(json.GetProperty("data"), ["id"]);
    }

    [Fact]
    public async Task Resposta400DeValidacao_DeveUsarCamelCase()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());

        var resposta = await EnviarJsonCruAsync(CriarCliente(_usuarioId), HttpMethod.Post, Rotas.Postagem.Base, """{"descricao":""}""");

        resposta.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var json = await resposta.LerJsonAsync();
        DeveTerExatamenteAsChaves(json, ChavesResposta);
        DeveTerExatamenteAsChaves(json.GetProperty("data").EnumerateArray().First(), ChavesErroValidacao);
    }

    [Fact]
    public async Task Resposta400DeFormato_DeveUsarCamelCase()
    {
        var resposta = await EnviarJsonCruAsync(CriarCliente(_usuarioId), HttpMethod.Post, Rotas.Postagem.Base, "{");

        resposta.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var json = await resposta.LerJsonAsync();
        DeveTerExatamenteAsChaves(json, ChavesResposta);
        DeveTerExatamenteAsChaves(json.GetProperty("data").EnumerateArray().First(), ChavesErroValidacao);
    }

    [Fact]
    public async Task Resposta401_DeveUsarCamelCase()
    {
        var resposta = await PatchAsync(CriarCliente(), Rotas.Perfil.Desativar);

        resposta.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        DeveTerExatamenteAsChaves(await resposta.LerJsonAsync(), ChavesResposta);
    }

    [Fact]
    public async Task Resposta401DeUsuarioNaoIdentificado_DeveUsarCamelCase()
    {
        var resposta = await PatchAsync(CriarClienteComToken(Factory.Tokens.GerarSemSub()), Rotas.Perfil.Desativar);

        resposta.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        DeveTerExatamenteAsChaves(await resposta.LerJsonAsync(), ChavesResposta);
    }

    [Fact]
    public async Task Resposta404DeDominio_DeveUsarCamelCase()
    {
        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Perfil.Desativar);

        resposta.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        resposta.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");
        DeveTerExatamenteAsChaves(await resposta.LerJsonAsync(), ChavesResposta);
    }

    private static void DeveTerExatamenteAsChaves(JsonElement objeto, string[] chaves)
    {
        objeto.ValueKind.ShouldBe(JsonValueKind.Object);
        objeto.EnumerateObject().Select(p => p.Name).ShouldBe(chaves, ignoreOrder: true);
    }
}
