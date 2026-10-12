using System.Net;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Perfil;

public class DeixarDeSeguirPerfilTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private readonly Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public async Task DeixarDeSeguir_QuandoSegue_DeveRetornar204RemoverVinculoEDecrementarTotais()
    {
        var (seguidor, seguido) = await SemearVinculoAsync();

        var resposta = await DeleteAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(seguido.Id));

        await resposta.DeveSerNoContentAsync();
        (await ConsultarAsync(db => db.PerfilSeguidor.CountAsync(Ct))).ShouldBe(0);
        (await ObterPerfilAsync(seguidor.Id)).TotalSeguindo.Total.ShouldBe(0);
        (await ObterPerfilAsync(seguido.Id)).TotalSeguidores.Total.ShouldBe(0);
    }

    [Fact]
    public async Task DeixarDeSeguirESeguirNovamente_DeveRecriarVinculo()
    {
        var (_, seguido) = await SemearVinculoAsync();
        var cliente = CriarCliente(_usuarioId);

        await (await DeleteAsync(cliente, Rotas.Perfil.Seguir(seguido.Id))).DeveSerNoContentAsync();
        await (await PostAsync(cliente, Rotas.Perfil.Seguir(seguido.Id))).DeveSerNoContentAsync();

        (await ConsultarAsync(db => db.PerfilSeguidor.CountAsync(Ct))).ShouldBe(1);
        (await ObterPerfilAsync(seguido.Id)).TotalSeguidores.Total.ShouldBe(1);
    }

    [Fact]
    public async Task DeixarDeSeguir_QuandoUsuarioNaoPossuiPerfil_DeveRetornar404()
    {
        var seguido = PerfilBuilder.Startup().Build();
        await SemearAsync(seguido);

        var resposta = await DeleteAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(seguido.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
    }

    [Fact]
    public async Task DeixarDeSeguir_QuandoNaoSegue_DeveRetornar404()
    {
        var seguidor = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build();
        var seguido = PerfilBuilder.Startup().Build();
        await SemearAsync(seguidor, seguido);

        var resposta = await DeleteAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(seguido.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Você não segue este perfil.");
    }

    [Fact]
    public async Task DeixarDeSeguir_QuandoOutroPerfilSegueOAlvo_NaoDeveRemoverVinculoAlheio()
    {
        var outroSeguidor = PerfilBuilder.Investidor().ComTotalSeguindo(1).Build();
        var seguido = PerfilBuilder.Startup().ComTotalSeguidores(1).Build();
        var eu = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(PerfilSeguidorBuilder.Novo().ComSeguidor(outroSeguidor).ComSeguido(seguido).Build(), eu);

        var resposta = await DeleteAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(seguido.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Você não segue este perfil.");
        (await ConsultarAsync(db => db.PerfilSeguidor.CountAsync(Ct))).ShouldBe(1);
        (await ObterPerfilAsync(seguido.Id)).TotalSeguidores.Total.ShouldBe(1);
    }

    [Fact]
    public async Task DeixarDeSeguir_QuandoPerfilSeguidoDesativado_DeveRetornar422EManterVinculo()
    {
        var (_, seguido) = await SemearVinculoAsync(seguidoDesativado: true);

        var resposta = await DeleteAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(seguido.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        (await ConsultarAsync(db => db.PerfilSeguidor.CountAsync(Ct))).ShouldBe(1);
        (await ObterPerfilAsync(seguido.Id)).TotalSeguidores.Total.ShouldBe(1);
    }

    [Fact]
    public async Task DeixarDeSeguir_QuandoPerfilSeguidorDesativado_DeveRetornar422EManterVinculo()
    {
        var (seguidor, seguido) = await SemearVinculoAsync(seguidorDesativado: true);

        var resposta = await DeleteAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(seguido.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        (await ConsultarAsync(db => db.PerfilSeguidor.CountAsync(Ct))).ShouldBe(1);
        (await ObterPerfilAsync(seguidor.Id)).TotalSeguindo.Total.ShouldBe(1);
    }

    [Fact]
    public async Task DeixarDeSeguir_ComIdVazio_DeveRetornar400()
    {
        var resposta = await DeleteAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(Guid.Empty));

        await resposta.DeveSerErroDeValidacaoAsync("SeguidoId", CodigosErroConst.CampoObrigatorio);
    }

    private async Task<(PerfilEntity Seguidor, PerfilEntity Seguido)> SemearVinculoAsync(
        bool seguidorDesativado = false,
        bool seguidoDesativado = false)
    {
        var seguidorBuilder = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).ComTotalSeguindo(1);
        var seguidoBuilder = PerfilBuilder.Startup().ComTotalSeguidores(1);

        if (seguidorDesativado)
            seguidorBuilder.Desativado();
        if (seguidoDesativado)
            seguidoBuilder.Desativado();

        var seguidor = seguidorBuilder.Build();
        var seguido = seguidoBuilder.Build();
        await SemearAsync(PerfilSeguidorBuilder.Novo().ComSeguidor(seguidor).ComSeguido(seguido).Build());

        return (seguidor, seguido);
    }

    private Task<PerfilEntity> ObterPerfilAsync(Guid id) =>
        ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == id, Ct));
}
