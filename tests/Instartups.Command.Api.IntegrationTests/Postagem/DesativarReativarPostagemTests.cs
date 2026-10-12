using System.Net;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Postagem;

public class DesativarReativarPostagemTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private readonly Guid _usuarioId = Guid.NewGuid();

    #region Desativar

    [Fact]
    public async Task Desativar_QuandoAutorEPostagemAtiva_DeveRetornar204EDescontarCurtidasDoAutor()
    {
        var (autor, postagem) = await SemearCenarioAsync(curtidasAutor: 5, curtidasPostagem: 3);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Desativar(postagem.Id));

        await resposta.DeveSerNoContentAsync();
        var persistida = await ObterPostagemAsync(postagem.Id);
        persistida.Ativo.ShouldBeFalse();
        persistida.TotalCurtidas.Total.ShouldBe(3);
        (await ObterPerfilAsync(autor.Id)).TotalCurtidas.Total.ShouldBe(2);
    }

    [Fact]
    public async Task Desativar_QuandoNaoEhAutor_DeveRetornar403EManterPostagemAtiva()
    {
        var postagem = PostagemBuilder.Nova().Build();
        await SemearAsync(postagem, PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build());

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Desativar(postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.Forbidden, "Apenas o autor pode realizar esta ação na postagem.");
        (await ObterPostagemAsync(postagem.Id)).Ativo.ShouldBeTrue();
    }

    [Fact]
    public async Task Desativar_QuandoPostagemNaoExiste_DeveRetornar404()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Desativar(Guid.NewGuid()));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Postagem não encontrada.");
    }

    [Fact]
    public async Task Desativar_QuandoUsuarioNaoPossuiPerfil_DeveRetornar404()
    {
        var postagem = PostagemBuilder.Nova().Build();
        await SemearAsync(postagem);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Desativar(postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
    }

    [Fact]
    public async Task Desativar_QuandoPostagemJaDesativada_DeveRetornar409ENaoDescontarCurtidas()
    {
        var (autor, postagem) = await SemearCenarioAsync(curtidasAutor: 2, curtidasPostagem: 3, postagemDesativada: true);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Desativar(postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.Conflict, "A postagem já está desativada.");
        (await ObterPerfilAsync(autor.Id)).TotalCurtidas.Total.ShouldBe(2);
    }

    [Fact]
    public async Task Desativar_QuandoPerfilDesativado_DeveRetornar422()
    {
        var (_, postagem) = await SemearCenarioAsync(autorDesativado: true);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Desativar(postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        (await ObterPostagemAsync(postagem.Id)).Ativo.ShouldBeTrue();
    }

    [Fact]
    public async Task Desativar_ComPostagemIdVazio_DeveRetornar400()
    {
        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Desativar(Guid.Empty));

        await resposta.DeveSerErroDeValidacaoAsync("PostagemId", CodigosErroConst.CampoObrigatorio);
    }

    #endregion

    #region Reativar

    [Fact]
    public async Task Reativar_QuandoAutorEPostagemDesativada_DeveRetornar204EDevolverCurtidasAoAutor()
    {
        var (autor, postagem) = await SemearCenarioAsync(curtidasAutor: 2, curtidasPostagem: 3, postagemDesativada: true);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Reativar(postagem.Id));

        await resposta.DeveSerNoContentAsync();
        (await ObterPostagemAsync(postagem.Id)).Ativo.ShouldBeTrue();
        (await ObterPerfilAsync(autor.Id)).TotalCurtidas.Total.ShouldBe(5);
    }

    [Fact]
    public async Task Reativar_QuandoPostagemJaAtiva_DeveRetornar409ENaoSomarCurtidas()
    {
        var (autor, postagem) = await SemearCenarioAsync(curtidasAutor: 5, curtidasPostagem: 3);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Reativar(postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.Conflict, "A postagem já está ativa.");
        (await ObterPerfilAsync(autor.Id)).TotalCurtidas.Total.ShouldBe(5);
    }

    [Fact]
    public async Task Reativar_QuandoNaoEhAutor_DeveRetornar403()
    {
        var postagem = PostagemBuilder.Nova().Desativada().Build();
        await SemearAsync(postagem, PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build());

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Reativar(postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.Forbidden, "Apenas o autor pode realizar esta ação na postagem.");
        (await ObterPostagemAsync(postagem.Id)).Ativo.ShouldBeFalse();
    }

    [Fact]
    public async Task Reativar_QuandoPostagemNaoExiste_DeveRetornar404()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Reativar(Guid.NewGuid()));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Postagem não encontrada.");
    }

    [Fact]
    public async Task Reativar_QuandoUsuarioNaoPossuiPerfil_DeveRetornar404()
    {
        var postagem = PostagemBuilder.Nova().Desativada().Build();
        await SemearAsync(postagem);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Reativar(postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
    }

    [Fact]
    public async Task Reativar_QuandoPerfilDesativado_DeveRetornar422()
    {
        var (_, postagem) = await SemearCenarioAsync(autorDesativado: true, postagemDesativada: true);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Reativar(postagem.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        (await ObterPostagemAsync(postagem.Id)).Ativo.ShouldBeFalse();
    }

    [Fact]
    public async Task Reativar_ComPostagemIdVazio_DeveRetornar400()
    {
        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Postagem.Reativar(Guid.Empty));

        await resposta.DeveSerErroDeValidacaoAsync("PostagemId", CodigosErroConst.CampoObrigatorio);
    }

    #endregion

    [Fact]
    public async Task DesativarEReativar_EmSequencia_DeveRestaurarTotalDeCurtidasDoAutor()
    {
        var (autor, postagem) = await SemearCenarioAsync(curtidasAutor: 7, curtidasPostagem: 4);
        var cliente = CriarCliente(_usuarioId);

        await (await PatchAsync(cliente, Rotas.Postagem.Desativar(postagem.Id))).DeveSerNoContentAsync();
        (await ObterPerfilAsync(autor.Id)).TotalCurtidas.Total.ShouldBe(3);

        await (await PatchAsync(cliente, Rotas.Postagem.Reativar(postagem.Id))).DeveSerNoContentAsync();
        (await ObterPerfilAsync(autor.Id)).TotalCurtidas.Total.ShouldBe(7);
    }

    private async Task<(PerfilEntity Autor, PostagemEntity Postagem)> SemearCenarioAsync(
        long curtidasAutor = 0,
        long curtidasPostagem = 0,
        bool autorDesativado = false,
        bool postagemDesativada = false)
    {
        var autorBuilder = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).ComTotalCurtidas(curtidasAutor);
        if (autorDesativado)
            autorBuilder.Desativado();
        var autor = autorBuilder.Build();

        var postagemBuilder = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(curtidasPostagem);
        if (postagemDesativada)
            postagemBuilder.Desativada();
        var postagem = postagemBuilder.Build();

        await SemearAsync(postagem);
        return (autor, postagem);
    }

    private Task<PostagemEntity> ObterPostagemAsync(Guid id) =>
        ConsultarAsync(db => db.Postagem.SingleAsync(p => p.Id == id, Ct));

    private Task<PerfilEntity> ObterPerfilAsync(Guid id) =>
        ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == id, Ct));
}
