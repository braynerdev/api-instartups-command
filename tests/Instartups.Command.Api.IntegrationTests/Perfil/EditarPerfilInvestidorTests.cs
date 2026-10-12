using System.Net;
using Instartups.Command.Api.DTOs;
using Instartups.Command.Api.DTOs.Perfil.Investidor;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Api.UnitTests.Builders;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Perfil;

public class EditarPerfilInvestidorTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private readonly Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public async Task EditarInvestidor_ComDadosValidos_DeveRetornar204EAtualizarPerfilEInvestidor()
    {
        var perfilOriginal = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(perfilOriginal);
        var dto = DtoBuilder.EditarInvestidor();

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Perfil.Investidor, dto);

        await resposta.DeveSerNoContentAsync();
        var perfil = await ObterPerfilAsync(perfilOriginal.Id);
        perfil.Nome.Nome.ShouldBe(dto.Perfil.Nome);
        perfil.ImagemPerfilUrl.ShouldBeNull();
        perfil.ImagemFundoUrl.ShouldBe(dto.Perfil.ImagemFundoUrl);
        perfil.Coordenada.Latitude.ShouldBe(dto.Perfil.Coordenada.Latitude, 1e-9);
        perfil.Coordenada.Longitude.ShouldBe(dto.Perfil.Coordenada.Longitude, 1e-9);
        perfil.UsuarioId.ShouldBe(_usuarioId);
        perfil.DataAtualizacao.ShouldNotBeNull();

        var investidor = perfil.Investidor.ShouldNotBeNull();
        investidor.Id.ShouldBe(perfilOriginal.Investidor!.Id);
        investidor.TeseInvestimento.ShouldBe(dto.TeseInvestimento);
        investidor.TicketMinimo.ShouldBe(dto.TicketMinimo);
        investidor.TicketMaximo.ShouldBe(dto.TicketMaximo);
        investidor.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public async Task EditarInvestidor_QuandoUsuarioNaoPossuiPerfil_DeveRetornar404()
    {
        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Perfil.Investidor, DtoBuilder.EditarInvestidor());

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
    }

    [Fact]
    public async Task EditarInvestidor_QuandoPerfilEhStartup_DeveRetornar422ENaoAlterarPerfil()
    {
        var startup = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(startup);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Perfil.Investidor, DtoBuilder.EditarInvestidor());

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "O perfil não é do tipo INVESTIDOR.");
        var perfil = await ObterPerfilAsync(startup.Id);
        perfil.Nome.Nome.ShouldBe(startup.Nome.Nome);
        perfil.Investidor.ShouldBeNull();
    }

    [Fact]
    public async Task EditarInvestidor_QuandoPerfilDesativado_DeveRetornar422ENaoAlterarPerfil()
    {
        var desativado = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Desativado().Build();
        await SemearAsync(desativado);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Perfil.Investidor, DtoBuilder.EditarInvestidor());

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        var perfil = await ObterPerfilAsync(desativado.Id);
        perfil.Nome.Nome.ShouldBe(desativado.Nome.Nome);
        perfil.Investidor!.TeseInvestimento.ShouldBe(desativado.Investidor!.TeseInvestimento);
    }

    [Theory]
    [InlineData("nome-vazio", "Perfil.Nome", CodigosErroConst.CampoObrigatorio)]
    [InlineData("imagem-fundo-invalida", "Perfil.ImagemFundoUrl", CodigosErroConst.UrlInvalida)]
    [InlineData("longitude-fora", "Perfil.Coordenada.Longitude", CodigosErroConst.ValorEntre)]
    [InlineData("tese-vazia", "TeseInvestimento", CodigosErroConst.CampoObrigatorio)]
    [InlineData("ticket-maximo-igual-minimo", "TicketMaximo", CodigosErroConst.ValorMinimo)]
    public async Task EditarInvestidor_ComPayloadInvalido_DeveRetornar400ENaoAlterarPerfil(string cenario, string campo, string codigo)
    {
        var original = PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(original);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Perfil.Investidor, PayloadInvalido(cenario));

        await resposta.DeveSerErroDeValidacaoAsync(campo, codigo);
        (await ObterPerfilAsync(original.Id)).DataAtualizacao.ShouldBeNull();
    }

    private Task<PerfilEntity> ObterPerfilAsync(Guid id) =>
        ConsultarAsync(db => db.Perfil.Include(p => p.Investidor).SingleAsync(p => p.Id == id, Ct));

    private static EditarPerfilInvestidorDTO PayloadInvalido(string cenario)
    {
        var valido = DtoBuilder.EditarInvestidor();
        var perfil = DtoBuilder.EditarPerfil();

        return cenario switch
        {
            "nome-vazio" => valido with { Perfil = perfil with { Nome = " " } },
            "imagem-fundo-invalida" => valido with { Perfil = perfil with { ImagemFundoUrl = "cdn.instartups.com/fundo.png" } },
            "longitude-fora" => valido with { Perfil = perfil with { Coordenada = new CoordenadaDTO(-8.0476, 180.5) } },
            "tese-vazia" => valido with { TeseInvestimento = "" },
            "ticket-maximo-igual-minimo" => valido with { TicketMinimo = 100_000m, TicketMaximo = 100_000m },
            _ => throw new ArgumentOutOfRangeException(nameof(cenario), cenario, null)
        };
    }
}
