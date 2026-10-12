using System.Net;
using Instartups.Command.Api.DTOs;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Perfil;

public class EditarCoordenadaPerfilTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private readonly Guid _usuarioId = Guid.NewGuid();
    private static readonly CoordenadaDTO Recife = new(-8.0476, -34.8770);

    [Fact]
    public async Task EditarCoordenada_ComCoordenadaValida_DeveRetornar204EPersistirNovaCoordenada()
    {
        var original = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(original);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Perfil.Coordenada, Recife);

        await resposta.DeveSerNoContentAsync();
        var perfil = await ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == original.Id, Ct));
        perfil.Coordenada.Latitude.ShouldBe(Recife.Latitude, 1e-9);
        perfil.Coordenada.Longitude.ShouldBe(Recife.Longitude, 1e-9);
        perfil.Nome.Nome.ShouldBe(original.Nome.Nome);
        perfil.DataAtualizacao.ShouldNotBeNull();
    }

    [Theory]
    [InlineData(-90, -180)]
    [InlineData(90, 180)]
    public async Task EditarCoordenada_NosLimitesDoGlobo_DeveRetornar204(double latitude, double longitude)
    {
        var original = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(original);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Perfil.Coordenada, new CoordenadaDTO(latitude, longitude));

        await resposta.DeveSerNoContentAsync();
        var perfil = await ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == original.Id, Ct));
        perfil.Coordenada.Latitude.ShouldBe(latitude, 1e-9);
        perfil.Coordenada.Longitude.ShouldBe(longitude, 1e-9);
    }

    [Fact]
    public async Task EditarCoordenada_QuandoUsuarioNaoPossuiPerfil_DeveRetornar404()
    {
        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Perfil.Coordenada, Recife);

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
    }

    [Fact]
    public async Task EditarCoordenada_QuandoPerfilDesativado_DeveRetornar422ENaoAlterarCoordenada()
    {
        var desativado = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Desativado().Build();
        await SemearAsync(desativado);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Perfil.Coordenada, Recife);

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        var perfil = await ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == desativado.Id, Ct));
        perfil.Coordenada.Latitude.ShouldBe(desativado.Coordenada.Latitude, 1e-9);
        perfil.Coordenada.Longitude.ShouldBe(desativado.Coordenada.Longitude, 1e-9);
    }

    [Theory]
    [InlineData(90.1, -34.877, "Coordenada.Latitude")]
    [InlineData(-91, -34.877, "Coordenada.Latitude")]
    [InlineData(-8.0476, 180.1, "Coordenada.Longitude")]
    [InlineData(-8.0476, -181, "Coordenada.Longitude")]
    public async Task EditarCoordenada_ForaDoIntervalo_DeveRetornar400(double latitude, double longitude, string campo)
    {
        var original = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(original);

        var resposta = await PatchAsync(CriarCliente(_usuarioId), Rotas.Perfil.Coordenada, new CoordenadaDTO(latitude, longitude));

        await resposta.DeveSerErroDeValidacaoAsync(campo, CodigosErroConst.ValorEntre);
        (await ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == original.Id, Ct))).DataAtualizacao.ShouldBeNull();
    }
}
