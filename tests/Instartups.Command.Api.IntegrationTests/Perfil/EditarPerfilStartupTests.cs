using System.Net;
using Instartups.Command.Api.DTOs.Perfil.Startup;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Api.UnitTests.Builders;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Perfil;

public class EditarPerfilStartupTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private readonly Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public async Task EditarStartup_ComDadosValidos_DeveRetornar204EAtualizarPerfilEStartup()
    {
        var perfilOriginal = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(perfilOriginal);
        var dto = DtoBuilder.EditarStartup();

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Perfil.Startup, dto);

        await resposta.DeveSerNoContentAsync();
        var perfil = await ObterPerfilAsync(perfilOriginal.Id);
        perfil.Nome.Nome.ShouldBe(dto.Perfil.Nome);
        perfil.ImagemPerfilUrl.ShouldBeNull();
        perfil.ImagemFundoUrl.ShouldBe(dto.Perfil.ImagemFundoUrl);
        perfil.Coordenada.Latitude.ShouldBe(dto.Perfil.Coordenada.Latitude, 1e-9);
        perfil.Coordenada.Longitude.ShouldBe(dto.Perfil.Coordenada.Longitude, 1e-9);
        perfil.DataAtualizacao.ShouldNotBeNull();

        var startup = perfil.Startup.ShouldNotBeNull();
        startup.Id.ShouldBe(perfilOriginal.Startup!.Id);
        startup.Pitch.ShouldBe(dto.Pitch);
        startup.DataFundacao.ShouldBe(dto.DataFundacao);
        startup.TamanhoEquipe.ShouldBe(dto.TamanhoEquipe);
        startup.ValorBuscado.ShouldBe(dto.ValorBuscado);
        startup.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public async Task EditarStartup_QuandoUsuarioNaoPossuiPerfil_DeveRetornar404()
    {
        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Perfil.Startup, DtoBuilder.EditarStartup());

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
    }

    [Fact]
    public async Task EditarStartup_QuandoPerfilEhInvestidor_DeveRetornar422ENaoAlterarPerfil()
    {
        var investidor = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(investidor);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Perfil.Startup, DtoBuilder.EditarStartup());

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "O perfil não é do tipo STARTUP.");
        var perfil = await ObterPerfilAsync(investidor.Id);
        perfil.Nome.Nome.ShouldBe(investidor.Nome.Nome);
        perfil.Startup.ShouldBeNull();
    }

    [Fact]
    public async Task EditarStartup_QuandoPerfilDesativado_DeveRetornar422ENaoAlterarPerfil()
    {
        var desativado = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Desativado().Build();
        await SemearAsync(desativado);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Perfil.Startup, DtoBuilder.EditarStartup());

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        var perfil = await ObterPerfilAsync(desativado.Id);
        perfil.Nome.Nome.ShouldBe(desativado.Nome.Nome);
        perfil.Startup!.Pitch.ShouldBe(desativado.Startup!.Pitch);
    }

    [Theory]
    [InlineData("nome-longo", "Perfil.Nome", CodigosErroConst.TamanhoMaximo)]
    [InlineData("pitch-vazio", "Pitch", CodigosErroConst.CampoObrigatorio)]
    [InlineData("data-futura", "DataFundacao", CodigosErroConst.DataFutura)]
    [InlineData("equipe-negativa", "TamanhoEquipe", CodigosErroConst.ValorEntre)]
    [InlineData("valor-zero", "ValorBuscado", CodigosErroConst.CampoObrigatorio)]
    public async Task EditarStartup_ComPayloadInvalido_DeveRetornar400ENaoAlterarPerfil(string cenario, string campo, string codigo)
    {
        var original = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(original);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Perfil.Startup, PayloadInvalido(cenario));

        await resposta.DeveSerErroDeValidacaoAsync(campo, codigo);
        (await ObterPerfilAsync(original.Id)).DataAtualizacao.ShouldBeNull();
    }

    private Task<PerfilEntity> ObterPerfilAsync(Guid id) =>
        ConsultarAsync(db => db.Perfil.Include(p => p.Startup).SingleAsync(p => p.Id == id, Ct));

    private static EditarPerfilStartupDTO PayloadInvalido(string cenario)
    {
        var valido = DtoBuilder.EditarStartup();

        return cenario switch
        {
            "nome-longo" => valido with { Perfil = DtoBuilder.EditarPerfil() with { Nome = new string('a', TamanhosColunasConst.Perfil.Nome + 1) } },
            "pitch-vazio" => valido with { Pitch = "" },
            "data-futura" => valido with { DataFundacao = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1) },
            "equipe-negativa" => valido with { TamanhoEquipe = -1 },
            "valor-zero" => valido with { ValorBuscado = 0m },
            _ => throw new ArgumentOutOfRangeException(nameof(cenario), cenario, null)
        };
    }
}
