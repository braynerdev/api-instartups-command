using System.Net;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Postagem;

public class CurtirDescurtirPostagemTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private readonly Guid _usuarioId = Guid.NewGuid();

    #region Curtir

    [Fact]
    public async Task Curtir_ComPerfisEPostagemAtivos_DeveRetornar204ECriarCurtidaEIncrementarTotais()
    {
        var cenario = await SemearCenarioAsync();

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(cenario.Postagem.Id));

        await resposta.DeveSerNoContentAsync();
        var curtida = await ConsultarAsync(db => db.CurtidaPostagem.SingleAsync(Ct));
        curtida.PostagemId.ShouldBe(cenario.Postagem.Id);
        curtida.PerfilAutorCurtidaId.ShouldBe(cenario.Curtidor.Id);

        (await ObterPostagemAsync(cenario.Postagem.Id)).TotalCurtidas.Total.ShouldBe(1);
        (await ObterPerfilAsync(cenario.Autor.Id)).TotalCurtidas.Total.ShouldBe(1);
        (await ObterPerfilAsync(cenario.Curtidor.Id)).TotalCurtidas.Total.ShouldBe(0);
    }

    [Fact]
    public async Task Curtir_PorDoisPerfisDiferentes_DeveSomarAsDuasCurtidas()
    {
        var cenario = await SemearCenarioAsync();
        var outroUsuarioId = Guid.NewGuid();
        await SemearAsync(PerfilBuilder.Investidor().ComUsuarioId(outroUsuarioId).Build());

        await (await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(cenario.Postagem.Id))).DeveSerNoContentAsync();
        await (await PostAsync(CriarCliente(outroUsuarioId), Rotas.Postagem.Curtir(cenario.Postagem.Id))).DeveSerNoContentAsync();

        (await ConsultarAsync(db => db.CurtidaPostagem.CountAsync(Ct))).ShouldBe(2);
        (await ObterPostagemAsync(cenario.Postagem.Id)).TotalCurtidas.Total.ShouldBe(2);
        (await ObterPerfilAsync(cenario.Autor.Id)).TotalCurtidas.Total.ShouldBe(2);
    }

    [Fact]
    public async Task Curtir_QuandoJaCurtiu_DeveRetornar409ENaoAlterarTotais()
    {
        var cenario = await SemearCenarioAsync(jaCurtiu: true);

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(cenario.Postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.Conflict, "Você já curtiu esta postagem.");
        (await ConsultarAsync(db => db.CurtidaPostagem.CountAsync(Ct))).ShouldBe(1);
        (await ObterPostagemAsync(cenario.Postagem.Id)).TotalCurtidas.Total.ShouldBe(1);
        (await ObterPerfilAsync(cenario.Autor.Id)).TotalCurtidas.Total.ShouldBe(1);
    }

    [Fact]
    public async Task Curtir_PropriaPostagem_DeveRetornar422()
    {
        var autor = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).Build();
        await SemearAsync(postagem);

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Um perfil não pode curtir a própria postagem.");
        (await ConsultarAsync(db => db.CurtidaPostagem.CountAsync(Ct))).ShouldBe(0);
        (await ObterPostagemAsync(postagem.Id)).TotalCurtidas.Total.ShouldBe(0);
    }

    [Fact]
    public async Task Curtir_QuandoPostagemDesativada_DeveRetornar422()
    {
        var cenario = await SemearCenarioAsync(postagemDesativada: true);

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(cenario.Postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque a postagem está desativada.");
        (await ConsultarAsync(db => db.CurtidaPostagem.CountAsync(Ct))).ShouldBe(0);
    }

    [Fact]
    public async Task Curtir_QuandoCurtidorDesativado_DeveRetornar422()
    {
        var cenario = await SemearCenarioAsync(curtidorDesativado: true);

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(cenario.Postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        (await ConsultarAsync(db => db.CurtidaPostagem.CountAsync(Ct))).ShouldBe(0);
    }

    [Fact]
    public async Task Curtir_QuandoAutorDaPostagemDesativado_DeveRetornar422()
    {
        var cenario = await SemearCenarioAsync(autorDesativado: true);

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(cenario.Postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        (await ConsultarAsync(db => db.CurtidaPostagem.CountAsync(Ct))).ShouldBe(0);
    }

    [Fact]
    public async Task Curtir_QuandoPostagemNaoExiste_DeveRetornar404()
    {
        await SemearAsync(PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build());

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(Guid.NewGuid()));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Postagem não encontrada.");
    }

    [Fact]
    public async Task Curtir_QuandoUsuarioNaoPossuiPerfil_DeveRetornar404()
    {
        var postagem = PostagemBuilder.Nova().Build();
        await SemearAsync(postagem);

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
    }

    [Fact]
    public async Task Curtir_ComPostagemIdVazio_DeveRetornar400()
    {
        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(Guid.Empty));

        await resposta.DeveSerErroDeValidacaoAsync("PostagemId", CodigosErroConst.CampoObrigatorio);
    }

    #endregion

    #region Descurtir

    [Fact]
    public async Task Descurtir_QuandoCurtiu_DeveRetornar204RemoverCurtidaEDecrementarTotais()
    {
        var cenario = await SemearCenarioAsync(jaCurtiu: true);

        var resposta = await DeleteAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(cenario.Postagem.Id));

        await resposta.DeveSerNoContentAsync();
        (await ConsultarAsync(db => db.CurtidaPostagem.CountAsync(Ct))).ShouldBe(0);
        (await ObterPostagemAsync(cenario.Postagem.Id)).TotalCurtidas.Total.ShouldBe(0);
        (await ObterPerfilAsync(cenario.Autor.Id)).TotalCurtidas.Total.ShouldBe(0);
    }

    [Fact]
    public async Task CurtirDescurtirECurtirNovamente_DeveTerUmaCurtidaETotaisConsistentes()
    {
        var cenario = await SemearCenarioAsync();
        var cliente = CriarCliente(_usuarioId);
        var rota = Rotas.Postagem.Curtir(cenario.Postagem.Id);

        await (await PostAsync(cliente, rota)).DeveSerNoContentAsync();
        await (await DeleteAsync(cliente, rota)).DeveSerNoContentAsync();
        await (await PostAsync(cliente, rota)).DeveSerNoContentAsync();

        (await ConsultarAsync(db => db.CurtidaPostagem.CountAsync(Ct))).ShouldBe(1);
        (await ObterPostagemAsync(cenario.Postagem.Id)).TotalCurtidas.Total.ShouldBe(1);
        (await ObterPerfilAsync(cenario.Autor.Id)).TotalCurtidas.Total.ShouldBe(1);
    }

    [Fact]
    public async Task Descurtir_QuandoNaoCurtiu_DeveRetornar404()
    {
        var cenario = await SemearCenarioAsync();

        var resposta = await DeleteAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(cenario.Postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Você não curtiu esta postagem.");
    }

    [Fact]
    public async Task Descurtir_QuandoUsuarioNaoPossuiPerfil_DeveRetornar404()
    {
        var postagem = PostagemBuilder.Nova().Build();
        await SemearAsync(postagem);

        var resposta = await DeleteAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
    }

    [Fact]
    public async Task Descurtir_QuandoPostagemDesativada_DeveRetornar422EManterCurtida()
    {
        var cenario = await SemearCenarioAsync(jaCurtiu: true, postagemDesativada: true);

        var resposta = await DeleteAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(cenario.Postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque a postagem está desativada.");
        (await ConsultarAsync(db => db.CurtidaPostagem.CountAsync(Ct))).ShouldBe(1);
    }

    [Fact]
    public async Task Descurtir_QuandoCurtidorDesativado_DeveRetornar422EManterCurtida()
    {
        var cenario = await SemearCenarioAsync(jaCurtiu: true, curtidorDesativado: true);

        var resposta = await DeleteAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(cenario.Postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        (await ConsultarAsync(db => db.CurtidaPostagem.CountAsync(Ct))).ShouldBe(1);
    }

    [Fact]
    public async Task Descurtir_ComPostagemIdVazio_DeveRetornar400()
    {
        var resposta = await DeleteAsync(CriarCliente(_usuarioId), Rotas.Postagem.Curtir(Guid.Empty));

        await resposta.DeveSerErroDeValidacaoAsync("PostagemId", CodigosErroConst.CampoObrigatorio);
    }

    #endregion

    private sealed record Cenario(PerfilEntity Autor, PerfilEntity Curtidor, PostagemEntity Postagem);

    private async Task<Cenario> SemearCenarioAsync(
        bool jaCurtiu = false,
        bool postagemDesativada = false,
        bool autorDesativado = false,
        bool curtidorDesativado = false)
    {
        var total = jaCurtiu ? 1 : 0;

        var autorBuilder = PerfilBuilder.Startup().ComTotalCurtidas(total);
        if (autorDesativado)
            autorBuilder.Desativado();
        var autor = autorBuilder.Build();

        var curtidorBuilder = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId);
        if (curtidorDesativado)
            curtidorBuilder.Desativado();
        var curtidor = curtidorBuilder.Build();

        var postagemBuilder = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(total);
        if (postagemDesativada)
            postagemBuilder.Desativada();
        var postagem = postagemBuilder.Build();

        if (jaCurtiu)
            await SemearAsync(CurtidaPostagemBuilder.Nova().DaPostagem(postagem).DoPerfil(curtidor).Build());
        else
            await SemearAsync(postagem, curtidor);

        return new Cenario(autor, curtidor, postagem);
    }

    private Task<PostagemEntity> ObterPostagemAsync(Guid id) =>
        ConsultarAsync(db => db.Postagem.SingleAsync(p => p.Id == id, Ct));

    private Task<PerfilEntity> ObterPerfilAsync(Guid id) =>
        ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == id, Ct));
}
