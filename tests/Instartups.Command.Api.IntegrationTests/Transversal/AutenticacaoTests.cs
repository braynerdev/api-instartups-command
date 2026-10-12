using System.Net;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Transversal;

public class AutenticacaoTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private const string MensagemNaoAutenticado = "É necessário estar autenticado para acessar este recurso.";
    private const string MensagemUsuarioNaoIdentificado = "Não foi possível identificar o usuário autenticado.";

    public static TheoryData<string, string> Endpoints()
    {
        var id = Guid.NewGuid();

        return new TheoryData<string, string>
        {
            { "POST", Rotas.Perfil.Investidor },
            { "PUT", Rotas.Perfil.Investidor },
            { "POST", Rotas.Perfil.Startup },
            { "PUT", Rotas.Perfil.Startup },
            { "PATCH", Rotas.Perfil.Coordenada },
            { "PATCH", Rotas.Perfil.Desativar },
            { "PATCH", Rotas.Perfil.Reativar },
            { "POST", Rotas.Perfil.Seguir(id) },
            { "DELETE", Rotas.Perfil.Seguir(id) },
            { "POST", Rotas.Postagem.Base },
            { "PUT", Rotas.Postagem.PorId(id) },
            { "PATCH", Rotas.Postagem.Desativar(id) },
            { "PATCH", Rotas.Postagem.Reativar(id) },
            { "POST", Rotas.Postagem.Curtir(id) },
            { "DELETE", Rotas.Postagem.Curtir(id) }
        };
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public async Task Endpoint_SemToken_DeveRetornar401(string metodo, string rota)
    {
        var resposta = await CriarCliente().SendAsync(new HttpRequestMessage(new HttpMethod(metodo), rota), Ct);

        await resposta.DeveSerErroAsync(HttpStatusCode.Unauthorized, MensagemNaoAutenticado);
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public async Task Endpoint_ComTokenSemSub_DeveRetornar401(string metodo, string rota)
    {
        var cliente = CriarClienteComToken(Factory.Tokens.GerarSemSub());

        var resposta = await EnviarJsonCruAsync(cliente, new HttpMethod(metodo), rota, CorpoValido(metodo, rota));

        await resposta.DeveSerErroAsync(HttpStatusCode.Unauthorized, MensagemUsuarioNaoIdentificado);
    }

    [Fact]
    public async Task Requisicao_ComTokenExpirado_DeveRetornar401ComMensagemDeExpiracao()
    {
        var cliente = CriarClienteComToken(Factory.Tokens.GerarExpirado(Guid.NewGuid()));

        var resposta = await PatchAsync(cliente, Rotas.Perfil.Desativar);

        await resposta.DeveSerErroAsync(HttpStatusCode.Unauthorized, "O token de acesso expirou.");
    }

    [Fact]
    public async Task Requisicao_ComTokenDeOutroIssuer_DeveRetornar401()
    {
        var cliente = CriarClienteComToken(Factory.Tokens.GerarComIssuer(Guid.NewGuid(), "outro-emissor"));

        var resposta = await PatchAsync(cliente, Rotas.Perfil.Desativar);

        await resposta.DeveSerErroAsync(HttpStatusCode.Unauthorized, MensagemNaoAutenticado);
    }

    [Fact]
    public async Task Requisicao_ComTokenDeOutraAudience_DeveRetornar401()
    {
        var cliente = CriarClienteComToken(Factory.Tokens.GerarComAudience(Guid.NewGuid(), "outro-app"));

        var resposta = await PatchAsync(cliente, Rotas.Perfil.Desativar);

        await resposta.DeveSerErroAsync(HttpStatusCode.Unauthorized, MensagemNaoAutenticado);
    }

    [Fact]
    public async Task Requisicao_ComTokenAssinadoPorOutraChave_DeveRetornar401()
    {
        var cliente = CriarClienteComToken(Factory.Tokens.GerarComOutraChave(Guid.NewGuid()));

        var resposta = await PatchAsync(cliente, Rotas.Perfil.Desativar);

        await resposta.DeveSerErroAsync(HttpStatusCode.Unauthorized, MensagemNaoAutenticado);
    }

    [Fact]
    public async Task Requisicao_ComTokenMalformado_DeveRetornar401()
    {
        var cliente = CriarClienteComToken("isso.nao.eh-um-jwt");

        var resposta = await PatchAsync(cliente, Rotas.Perfil.Desativar);

        await resposta.DeveSerErroAsync(HttpStatusCode.Unauthorized, MensagemNaoAutenticado);
    }

    [Theory]
    [InlineData("nao-eh-guid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task Requisicao_ComSubInvalido_DeveRetornar401(string sub)
    {
        var cliente = CriarClienteComToken(Factory.Tokens.GerarComSub(sub));

        var resposta = await PatchAsync(cliente, Rotas.Perfil.Desativar);

        await resposta.DeveSerErroAsync(HttpStatusCode.Unauthorized, MensagemUsuarioNaoIdentificado);
    }

    [Fact]
    public async Task Requisicao_ComTokenValido_DeveUsarSubComoIdentificadorDoUsuario()
    {
        var usuarioId = Guid.NewGuid();
        var perfil = PerfilBuilder.Startup().ComUsuarioId(usuarioId).Build();
        await SemearAsync(perfil);

        var resposta = await PatchAsync(CriarCliente(usuarioId), Rotas.Perfil.Desativar);

        await resposta.DeveSerNoContentAsync();
        (await ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == perfil.Id, Ct))).Ativo.ShouldBeFalse();
    }

    /// <summary>
    /// Corpo que passa no model binding, para garantir que a falha vem da identificação do usuário.
    /// </summary>
    private static string CorpoValido(string metodo, string rota) => (metodo, rota) switch
    {
        (_, Rotas.Perfil.Investidor) =>
            """{"perfil":{"nome":"x","coordenada":{"latitude":-8.05,"longitude":-34.88}},"teseInvestimento":"x","ticketMinimo":1,"ticketMaximo":2}""",
        (_, Rotas.Perfil.Startup) =>
            """{"perfil":{"nome":"x","coordenada":{"latitude":-8.05,"longitude":-34.88}},"pitch":"x","dataFundacao":"2020-01-01","tamanhoEquipe":1,"valorBuscado":1}""",
        (_, Rotas.Perfil.Coordenada) => """{"latitude":-8.05,"longitude":-34.88}""",
        ("POST", Rotas.Postagem.Base) => """{"descricao":"x"}""",
        ("PUT", _) => """{"descricao":"x"}""",
        _ => "{}"
    };
}
