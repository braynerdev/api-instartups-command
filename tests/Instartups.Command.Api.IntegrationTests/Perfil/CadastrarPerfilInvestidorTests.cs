using System.Net;
using Instartups.Command.Api.DTOs;
using Instartups.Command.Api.DTOs.Perfil.Investidor;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Api.UnitTests.Builders;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Perfil;

public class CadastrarPerfilInvestidorTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private readonly Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public async Task CadastrarInvestidor_ComDadosValidos_DeveRetornar201EPersistirPerfilEInvestidor()
    {
        var dto = DtoBuilder.CadastrarInvestidor();

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Investidor, dto);

        var id = await resposta.DeveSerCriadoAsync(MensagensSucessoConst.PerfilCadastrado);
        var perfil = await ConsultarAsync(db => db.Perfil
            .Include(p => p.Investidor)
            .Include(p => p.Startup)
            .SingleAsync(p => p.Id == id, Ct));

        perfil.UsuarioId.ShouldBe(_usuarioId);
        perfil.TipoPerfil.ShouldBe(TiposPerfisEnum.Investidor);
        perfil.Nome.Nome.ShouldBe(dto.Perfil.Nome);
        perfil.ImagemPerfilUrl.ShouldBe(dto.Perfil.ImagemPerfilUrl);
        perfil.ImagemFundoUrl.ShouldBe(dto.Perfil.ImagemFundoUrl);
        perfil.Coordenada.Latitude.ShouldBe(dto.Perfil.Coordenada.Latitude, 1e-9);
        perfil.Coordenada.Longitude.ShouldBe(dto.Perfil.Coordenada.Longitude, 1e-9);
        perfil.TotalCurtidas.Total.ShouldBe(0);
        perfil.TotalSeguidores.Total.ShouldBe(0);
        perfil.TotalSeguindo.Total.ShouldBe(0);
        perfil.Ativo.ShouldBeTrue();
        perfil.DataAtualizacao.ShouldBeNull();
        perfil.Startup.ShouldBeNull();

        var investidor = perfil.Investidor.ShouldNotBeNull();
        investidor.PerfilId.ShouldBe(id);
        investidor.TeseInvestimento.ShouldBe(dto.TeseInvestimento);
        investidor.TicketMinimo.ShouldBe(dto.TicketMinimo);
        investidor.TicketMaximo.ShouldBe(dto.TicketMaximo);
    }

    [Fact]
    public async Task CadastrarInvestidor_SemImagens_DeveRetornar201EPersistirImagensNulas()
    {
        var dto = DtoBuilder.CadastrarInvestidor() with
        {
            Perfil = DtoBuilder.CadastrarPerfil() with { ImagemPerfilUrl = null, ImagemFundoUrl = null }
        };

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Investidor, dto);

        var id = await resposta.DeveSerCriadoAsync(MensagensSucessoConst.PerfilCadastrado);
        var perfil = await ConsultarAsync(db => db.Perfil.SingleAsync(p => p.Id == id, Ct));
        perfil.ImagemPerfilUrl.ShouldBeNull();
        perfil.ImagemFundoUrl.ShouldBeNull();
    }

    [Fact]
    public async Task CadastrarInvestidor_ComTicketsNoLimiteDaPrecisao_DeveRetornar201EPersistirValoresExatos()
    {
        var dto = DtoBuilder.CadastrarInvestidor() with { TicketMinimo = 0.01m, TicketMaximo = 9_999_999_999.99m };

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Investidor, dto);

        var id = await resposta.DeveSerCriadoAsync(MensagensSucessoConst.PerfilCadastrado);
        var investidor = await ConsultarAsync(db => db.Investidor.SingleAsync(i => i.PerfilId == id, Ct));
        investidor.TicketMinimo.ShouldBe(0.01m);
        investidor.TicketMaximo.ShouldBe(9_999_999_999.99m);
    }

    [Fact]
    public async Task CadastrarInvestidor_QuandoUsuarioJaPossuiPerfil_DeveRetornar409ENaoPersistirNovoPerfil()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Investidor, DtoBuilder.CadastrarInvestidor());

        await resposta.DeveSerErroAsync(HttpStatusCode.Conflict, "O usuário já possui um perfil cadastrado.");
        (await ConsultarAsync(db => db.Perfil.CountAsync(p => p.UsuarioId == _usuarioId, Ct))).ShouldBe(1);
        (await ConsultarAsync(db => db.Investidor.CountAsync(Ct))).ShouldBe(0);
    }

    [Theory]
    [InlineData("nome-vazio", "Perfil.Nome", CodigosErroConst.CampoObrigatorio)]
    [InlineData("nome-longo", "Perfil.Nome", CodigosErroConst.TamanhoMaximo)]
    [InlineData("imagem-perfil-invalida", "Perfil.ImagemPerfilUrl", CodigosErroConst.UrlInvalida)]
    [InlineData("imagem-fundo-longa", "Perfil.ImagemFundoUrl", CodigosErroConst.TamanhoMaximo)]
    [InlineData("latitude-fora", "Perfil.Coordenada.Latitude", CodigosErroConst.ValorEntre)]
    [InlineData("longitude-fora", "Perfil.Coordenada.Longitude", CodigosErroConst.ValorEntre)]
    [InlineData("tese-vazia", "TeseInvestimento", CodigosErroConst.CampoObrigatorio)]
    [InlineData("tese-longa", "TeseInvestimento", CodigosErroConst.TamanhoMaximo)]
    [InlineData("ticket-minimo-negativo", "TicketMinimo", CodigosErroConst.ValorMinimo)]
    [InlineData("ticket-minimo-tres-casas", "TicketMinimo", CodigosErroConst.PrecisaoIncorreta)]
    [InlineData("ticket-maximo-menor-que-minimo", "TicketMaximo", CodigosErroConst.ValorMinimo)]
    [InlineData("ticket-maximo-acima-da-precisao", "TicketMaximo", CodigosErroConst.PrecisaoIncorreta)]
    public async Task CadastrarInvestidor_ComPayloadInvalido_DeveRetornar400ENaoPersistir(string cenario, string campo, string codigo)
    {
        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Investidor, PayloadInvalido(cenario));

        await resposta.DeveSerErroDeValidacaoAsync(campo, codigo);
        (await ConsultarAsync(db => db.Perfil.CountAsync(Ct))).ShouldBe(0);
    }

    private static CadastrarPerfilInvestidorDTO PayloadInvalido(string cenario)
    {
        var valido = DtoBuilder.CadastrarInvestidor();
        var perfil = DtoBuilder.CadastrarPerfil();

        return cenario switch
        {
            "nome-vazio" => valido with { Perfil = perfil with { Nome = "" } },
            "nome-longo" => valido with { Perfil = perfil with { Nome = new string('a', TamanhosColunasConst.Perfil.Nome + 1) } },
            "imagem-perfil-invalida" => valido with { Perfil = perfil with { ImagemPerfilUrl = "ftp://cdn.instartups.com/perfil.png" } },
            "imagem-fundo-longa" => valido with { Perfil = perfil with { ImagemFundoUrl = "https://cdn.instartups.com/" + new string('a', TamanhosColunasConst.Perfil.ImagemFundoUrl) } },
            "latitude-fora" => valido with { Perfil = perfil with { Coordenada = new CoordenadaDTO(91, -34.877) } },
            "longitude-fora" => valido with { Perfil = perfil with { Coordenada = new CoordenadaDTO(-8.0476, -181) } },
            "tese-vazia" => valido with { TeseInvestimento = "" },
            "tese-longa" => valido with { TeseInvestimento = new string('a', TamanhosColunasConst.Investidor.Tese + 1) },
            "ticket-minimo-negativo" => valido with { TicketMinimo = -1m },
            "ticket-minimo-tres-casas" => valido with { TicketMinimo = 1_000.123m },
            "ticket-maximo-menor-que-minimo" => valido with { TicketMinimo = 500_000m, TicketMaximo = 100_000m },
            "ticket-maximo-acima-da-precisao" => valido with { TicketMaximo = 10_000_000_000m },
            _ => throw new ArgumentOutOfRangeException(nameof(cenario), cenario, null)
        };
    }
}
