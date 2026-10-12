using System.Net;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Perfil;

public class DesativarReativarPerfilTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private readonly Guid _usuarioId = Guid.NewGuid();

    #region Desativar

    [Fact]
    public async Task Desativar_QuandoPerfilAtivo_DeveRetornar204EDesativarPerfil()
    {
        var perfil = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(perfil);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Perfil.Desativar);

        await resposta.DeveSerNoContentAsync();
        var persistido = await ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == perfil.Id, Ct));
        persistido.Ativo.ShouldBeFalse();
        persistido.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public async Task Desativar_NaoDeveAfetarPerfilDeOutroUsuario()
    {
        var meu = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build();
        var outro = PerfilBuilder.Investidor().Build();
        await SemearAsync(meu, outro);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Perfil.Desativar);

        await resposta.DeveSerNoContentAsync();
        (await ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == outro.Id, Ct))).Ativo.ShouldBeTrue();
    }

    [Fact]
    public async Task Desativar_QuandoUsuarioNaoPossuiPerfil_DeveRetornar404()
    {
        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Perfil.Desativar);

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
    }

    [Fact]
    public async Task Desativar_QuandoPerfilJaDesativado_DeveRetornar409()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Desativado().Build());

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Perfil.Desativar);

        await resposta.DeveSerErroAsync(HttpStatusCode.Conflict, "O perfil já está desativado.");
    }

    #endregion

    #region Reativar

    [Fact]
    public async Task Reativar_QuandoPerfilDesativado_DeveRetornar204EReativarPerfil()
    {
        var perfil = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Desativado().Build();
        await SemearAsync(perfil);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Perfil.Reativar);

        await resposta.DeveSerNoContentAsync();
        (await ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == perfil.Id, Ct))).Ativo.ShouldBeTrue();
    }

    [Fact]
    public async Task Reativar_QuandoUsuarioNaoPossuiPerfil_DeveRetornar404()
    {
        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Perfil.Reativar);

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
    }

    [Fact]
    public async Task Reativar_QuandoPerfilJaAtivo_DeveRetornar409()
    {
        await SemearAsync(PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build());

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Perfil.Reativar);

        await resposta.DeveSerErroAsync(HttpStatusCode.Conflict, "O perfil já está ativo.");
    }

    #endregion

    [Fact]
    public async Task DesativarEReativar_EmSequencia_DeveVoltarAoEstadoAtivo()
    {
        var perfil = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(perfil);
        var cliente = CriarCliente(_usuarioId);

        await (await PatchAsync(cliente, Rotas.Perfil.Desativar)).DeveSerNoContentAsync();
        await (await PatchAsync(cliente, Rotas.Perfil.Reativar)).DeveSerNoContentAsync();

        (await ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == perfil.Id, Ct))).Ativo.ShouldBeTrue();
    }
}
