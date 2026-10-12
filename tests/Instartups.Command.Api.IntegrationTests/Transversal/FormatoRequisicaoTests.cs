using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Transversal;

public class FormatoRequisicaoTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private readonly Guid _usuarioId = Guid.NewGuid();

    [Theory]
    [InlineData("""{"descricao": """)]
    [InlineData("""{"descricao": "x", "midias": "nao-eh-lista"}""")]
    [InlineData("nao-eh-json")]
    public async Task Requisicao_ComJsonMalformado_DeveRetornar400ComFormatoInvalido(string json)
    {
        var resposta = await EnviarJsonCruAsync(CriarCliente(_usuarioId), HttpMethod.Post, Rotas.Postagem.Base, json);

        await DeveSerFormatoInvalidoAsync(resposta);
        (await ConsultarAsync(db => db.Postagem.CountAsync(Ct))).ShouldBe(0);
    }

    [Theory]
    [InlineData("""{"latitude": "norte", "longitude": -34.88}""")]
    [InlineData("""{"latitude": -8.05, "longitude": true}""")]
    public async Task Requisicao_ComTipoDeCampoErrado_DeveRetornar400ComFormatoInvalido(string json)
    {
        var resposta = await EnviarJsonCruAsync(CriarCliente(_usuarioId), HttpMethod.Patch, Rotas.Perfil.Coordenada, json);

        await DeveSerFormatoInvalidoAsync(resposta);
    }

    [Fact]
    public async Task Requisicao_ComDataEmFormatoInvalido_DeveRetornar400ComFormatoInvalido()
    {
        const string json = """
            {"perfil":{"nome":"x","coordenada":{"latitude":-8.05,"longitude":-34.88}},
             "pitch":"x","dataFundacao":"15/01/2020","tamanhoEquipe":1,"valorBuscado":1}
            """;

        var resposta = await EnviarJsonCruAsync(CriarCliente(_usuarioId), HttpMethod.Post, Rotas.Perfil.Startup, json);

        await DeveSerFormatoInvalidoAsync(resposta);
    }

    [Fact]
    public async Task Requisicao_ComContentTypeNaoJson_DeveRetornar415()
    {
        var requisicao = new HttpRequestMessage(HttpMethod.Post, Rotas.Postagem.Base)
        {
            Content = new StringContent("descricao=x", Encoding.UTF8, "text/plain")
        };

        var resposta = await CriarCliente(_usuarioId).SendAsync(requisicao, Ct);

        resposta.StatusCode.ShouldBe(HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async Task Requisicao_AceitandoSomenteXml_DeveRetornar406()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());
        var cliente = CriarCliente(_usuarioId);
        cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        var resposta = await EnviarJsonCruAsync(cliente, HttpMethod.Post, Rotas.Postagem.Base, """{"descricao":"x"}""");

        resposta.StatusCode.ShouldBe(HttpStatusCode.NotAcceptable);
    }

    [Theory]
    [InlineData("PATCH", "api/instartups/command/postagem/nao-eh-guid/desativar")]
    [InlineData("POST", "api/instartups/command/perfil/123/seguir")]
    [InlineData("GET", "api/instartups/command/perfil/investidor")]
    public async Task Requisicao_ParaRotaInexistente_DeveRetornar404Ou405(string metodo, string rota)
    {
        var resposta = await CriarCliente(_usuarioId).SendAsync(new HttpRequestMessage(new HttpMethod(metodo), rota), Ct);

        resposta.StatusCode.ShouldBeOneOf(HttpStatusCode.NotFound, HttpStatusCode.MethodNotAllowed);
    }

    [Fact]
    public async Task Requisicao_ComRotaEmMaiusculas_DeveSerAceita()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());

        var resposta = await PatchAsync(CriarCliente(_usuarioId), "API/Instartups/Command/Perfil/Desativar");

        await resposta.DeveSerNoContentAsync();
    }

    [Fact]
    public async Task Requisicao_ComCamposEmPascalCase_DeveSerAceita()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());

        var resposta = await EnviarJsonCruAsync(CriarCliente(_usuarioId), HttpMethod.Post, Rotas.Postagem.Base,
            """{"Descricao":"Campos em PascalCase.","Midias":[{"Url":"https://cdn.instartups.com/a.png","Tipo":"FOTO"}]}""");

        var id = await resposta.DeveSerCriadoAsync(MensagensSucessoConst.PostagemCadastrada);
        (await ConsultarAsync(db => db.MidiaPostagem.CountAsync(m => m.PostagemId == id, Ct))).ShouldBe(1);
    }

    #region Corpos nulos ou incompletos

    [Theory]
    [InlineData("")]
    [InlineData("null")]
    public async Task CadastrarPostagem_ComCorpoVazioOuNulo_DeveRetornar400(string json)
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());

        var resposta = await EnviarJsonCruAsync(CriarCliente(_usuarioId), HttpMethod.Post, Rotas.Postagem.Base, json);

        resposta.StatusCode.ShouldBe(HttpStatusCode.BadRequest, await resposta.Content.ReadAsStringAsync(Ct));
    }

    [Fact]
    public async Task CadastrarInvestidor_ComPerfilNulo_DeveRetornar400()
    {
        var resposta = await EnviarJsonCruAsync(CriarCliente(_usuarioId), HttpMethod.Post, Rotas.Perfil.Investidor,
            """{"perfil":null,"teseInvestimento":"x","ticketMinimo":1,"ticketMaximo":2}""");

        resposta.StatusCode.ShouldBe(HttpStatusCode.BadRequest, await resposta.Content.ReadAsStringAsync(Ct));
        (await ConsultarAsync(db => db.Perfil.CountAsync(Ct))).ShouldBe(0);
    }

    [Fact]
    public async Task EditarCoordenada_ComCorpoNulo_DeveRetornar400()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());

        var resposta = await EnviarJsonCruAsync(CriarCliente(_usuarioId), HttpMethod.Patch, Rotas.Perfil.Coordenada, "null");

        resposta.StatusCode.ShouldBe(HttpStatusCode.BadRequest, await resposta.Content.ReadAsStringAsync(Ct));
    }

    [Theory]
    [InlineData("""{"descricao":"x","midias":[{"url":"https://cdn.instartups.com/a.png","tipo":null}]}""")]
    [InlineData("""{"descricao":"x","midias":[{"url":"https://cdn.instartups.com/a.png"}]}""")]
    [InlineData("""{"descricao":"x","midias":[null]}""")]
    public async Task CadastrarPostagem_ComMidiaIncompleta_DeveRetornar400(string json)
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());

        var resposta = await EnviarJsonCruAsync(CriarCliente(_usuarioId), HttpMethod.Post, Rotas.Postagem.Base, json);

        resposta.StatusCode.ShouldBe(HttpStatusCode.BadRequest, await resposta.Content.ReadAsStringAsync(Ct));
        (await ConsultarAsync(db => db.Postagem.CountAsync(Ct))).ShouldBe(0);
    }

    [Fact]
    public async Task CadastrarPostagem_SemDescricao_DeveRetornar400ComCampoObrigatorio()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());

        var resposta = await EnviarJsonCruAsync(CriarCliente(_usuarioId), HttpMethod.Post, Rotas.Postagem.Base, "{}");

        await resposta.DeveSerErroDeValidacaoAsync("Descricao", CodigosErroConst.CampoObrigatorio);
    }

    #endregion

    private static async Task DeveSerFormatoInvalidoAsync(HttpResponseMessage resposta)
    {
        resposta.StatusCode.ShouldBe(HttpStatusCode.BadRequest, await resposta.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        var corpo = await resposta.LerCorpoAsync<List<ErroValidacao>>();
        corpo.IsSuccess.ShouldBeFalse();
        corpo.Message.ShouldBe(MensagensErroConst.FormatoInvalido);
        corpo.Data.ShouldNotBeNull().ShouldNotBeEmpty();
        corpo.Data.ShouldAllBe(e => e.Codigo == CodigosErroConst.FormatoInvalido && e.Mensagem == "Formato da requisição inválido.");
    }
}
