using System.Net;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Perfil;

public class SeguirPerfilTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private readonly Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public async Task Seguir_ComPerfisAtivos_DeveRetornar204ECriarVinculoEAtualizarTotais()
    {
        var seguidor = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build();
        var seguido = PerfilBuilder.Startup().Build();
        await SemearAsync(seguidor, seguido);

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(seguido.Id));

        await resposta.DeveSerNoContentAsync();
        var vinculo = await ConsultarAsync(db => db.PerfilSeguidor.SingleAsync(Ct));
        vinculo.SeguidorId.ShouldBe(seguidor.Id);
        vinculo.SeguidoId.ShouldBe(seguido.Id);
        vinculo.Ativo.ShouldBeTrue();

        var seguidorPersistido = await ObterPerfilAsync(seguidor.Id);
        seguidorPersistido.TotalSeguindo.Total.ShouldBe(1);
        seguidorPersistido.TotalSeguidores.Total.ShouldBe(0);

        var seguidoPersistido = await ObterPerfilAsync(seguido.Id);
        seguidoPersistido.TotalSeguidores.Total.ShouldBe(1);
        seguidoPersistido.TotalSeguindo.Total.ShouldBe(0);
    }

    [Fact]
    public async Task Seguir_QuandoAmbosSeSeguem_DeveCriarDoisVinculosIndependentes()
    {
        var outroUsuarioId = Guid.NewGuid();
        var perfilA = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build();
        var perfilB = PerfilBuilder.Startup().ComUsuarioId(outroUsuarioId).Build();
        await SemearAsync(perfilA, perfilB);

        await (await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(perfilB.Id))).DeveSerNoContentAsync();
        await (await PostAsync(CriarCliente(outroUsuarioId), Rotas.Perfil.Seguir(perfilA.Id))).DeveSerNoContentAsync();

        (await ConsultarAsync(db => db.PerfilSeguidor.CountAsync(Ct))).ShouldBe(2);
        foreach (var id in new[] { perfilA.Id, perfilB.Id })
        {
            var perfil = await ObterPerfilAsync(id);
            perfil.TotalSeguindo.Total.ShouldBe(1);
            perfil.TotalSeguidores.Total.ShouldBe(1);
        }
    }

    [Fact]
    public async Task Seguir_QuandoUsuarioNaoPossuiPerfil_DeveRetornar404()
    {
        var seguido = PerfilBuilder.Startup().Build();
        await SemearAsync(seguido);

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(seguido.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
        (await ConsultarAsync(db => db.PerfilSeguidor.CountAsync(Ct))).ShouldBe(0);
    }

    [Fact]
    public async Task Seguir_QuandoPerfilSeguidoNaoExiste_DeveRetornar404()
    {
        await SemearAsync(PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build());

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(Guid.NewGuid()));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
    }

    [Fact]
    public async Task Seguir_QuandoIdInformadoEhUsuarioIdEmVezDePerfilId_DeveRetornar404()
    {
        var outroUsuarioId = Guid.NewGuid();
        await SemearAsync(
            PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build(),
            PerfilBuilder.Startup().ComUsuarioId(outroUsuarioId).Build());

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(outroUsuarioId));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
    }

    [Fact]
    public async Task Seguir_QuandoJaSegue_DeveRetornar409ENaoAlterarTotais()
    {
        var seguidor = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).ComTotalSeguindo(1).Build();
        var seguido = PerfilBuilder.Startup().ComTotalSeguidores(1).Build();
        var vinculo = PerfilSeguidorBuilder.Novo().ComSeguidor(seguidor).ComSeguido(seguido).Build();
        await SemearAsync(vinculo);

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(seguido.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.Conflict, "Você já segue este perfil.");
        (await ConsultarAsync(db => db.PerfilSeguidor.CountAsync(Ct))).ShouldBe(1);
        (await ObterPerfilAsync(seguidor.Id)).TotalSeguindo.Total.ShouldBe(1);
        (await ObterPerfilAsync(seguido.Id)).TotalSeguidores.Total.ShouldBe(1);
    }

    [Fact]
    public async Task Seguir_ASiMesmo_DeveRetornar422()
    {
        var perfil = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(perfil);

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(perfil.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Um perfil não pode seguir a si mesmo.");
        (await ConsultarAsync(db => db.PerfilSeguidor.CountAsync(Ct))).ShouldBe(0);
        (await ObterPerfilAsync(perfil.Id)).TotalSeguindo.Total.ShouldBe(0);
    }

    [Fact]
    public async Task Seguir_QuandoPerfilSeguidoDesativado_DeveRetornar422()
    {
        var seguidor = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build();
        var seguido = PerfilBuilder.Startup().Desativado().Build();
        await SemearAsync(seguidor, seguido);

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(seguido.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        (await ConsultarAsync(db => db.PerfilSeguidor.CountAsync(Ct))).ShouldBe(0);
    }

    [Fact]
    public async Task Seguir_QuandoPerfilSeguidorDesativado_DeveRetornar422()
    {
        var seguidor = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Desativado().Build();
        var seguido = PerfilBuilder.Startup().Build();
        await SemearAsync(seguidor, seguido);

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(seguido.Id));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        (await ConsultarAsync(db => db.PerfilSeguidor.CountAsync(Ct))).ShouldBe(0);
    }

    [Fact]
    public async Task Seguir_ComIdVazio_DeveRetornar400()
    {
        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Seguir(Guid.Empty));

        await resposta.DeveSerErroDeValidacaoAsync("SeguidoId", CodigosErroConst.CampoObrigatorio);
    }

    private Task<PerfilEntity> ObterPerfilAsync(Guid id) =>
        ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == id, Ct));
}
