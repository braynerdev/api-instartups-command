using System.Net;
using Instartups.Command.Api.DTOs;
using Instartups.Command.Api.DTOs.Perfil.Startup;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Api.UnitTests.Builders;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Perfil;

public class CadastrarPerfilStartupTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private readonly Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public async Task CadastrarStartup_ComDadosValidos_DeveRetornar201EPersistirPerfilEStartup()
    {
        var dto = DtoBuilder.CadastrarStartup();

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Startup, dto);

        var id = await resposta.DeveSerCriadoAsync(MensagensSucessoConst.PerfilCadastrado);
        var perfil = await ConsultarAsync(db => db.Perfil
            .Include(p => p.Startup)
            .Include(p => p.Investidor)
            .SingleAsync(p => p.Id == id, Ct));

        perfil.UsuarioId.ShouldBe(_usuarioId);
        perfil.TipoPerfil.ShouldBe(TiposPerfisEnum.Startup);
        perfil.Nome.Nome.ShouldBe(dto.Perfil.Nome);
        perfil.ImagemPerfilUrl.ShouldBe(dto.Perfil.ImagemPerfilUrl);
        perfil.ImagemFundoUrl.ShouldBe(dto.Perfil.ImagemFundoUrl);
        perfil.Coordenada.Latitude.ShouldBe(dto.Perfil.Coordenada.Latitude, 1e-9);
        perfil.Coordenada.Longitude.ShouldBe(dto.Perfil.Coordenada.Longitude, 1e-9);
        perfil.TotalCurtidas.Total.ShouldBe(0);
        perfil.TotalSeguidores.Total.ShouldBe(0);
        perfil.TotalSeguindo.Total.ShouldBe(0);
        perfil.Ativo.ShouldBeTrue();
        perfil.Investidor.ShouldBeNull();

        var startup = perfil.Startup.ShouldNotBeNull();
        startup.PerfilId.ShouldBe(id);
        startup.Pitch.ShouldBe(dto.Pitch);
        startup.DataFundacao.ShouldBe(dto.DataFundacao);
        startup.TamanhoEquipe.ShouldBe(dto.TamanhoEquipe);
        startup.ValorBuscado.ShouldBe(dto.ValorBuscado);
        startup.Setor.ShouldBe(SetorStartupEnum.Financas);
    }

    [Fact]
    public async Task CadastrarStartup_FundadaHoje_DeveRetornar201()
    {
        var dto = DtoBuilder.CadastrarStartup() with { DataFundacao = DateOnly.FromDateTime(DateTime.UtcNow) };

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Startup, dto);

        var id = await resposta.DeveSerCriadoAsync(MensagensSucessoConst.PerfilCadastrado);
        var startup = await ConsultarAsync(db => db.Startup.SingleAsync(s => s.PerfilId == id, Ct));
        startup.DataFundacao.ShouldBe(dto.DataFundacao);
    }

    [Fact]
    public async Task CadastrarStartup_ComTamanhoEquipeMaximo_DeveRetornar201()
    {
        var dto = DtoBuilder.CadastrarStartup() with { TamanhoEquipe = short.MaxValue };

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Startup, dto);

        var id = await resposta.DeveSerCriadoAsync(MensagensSucessoConst.PerfilCadastrado);
        var startup = await ConsultarAsync(db => db.Startup.SingleAsync(s => s.PerfilId == id, Ct));
        startup.TamanhoEquipe.ShouldBe(short.MaxValue);
    }

    [Fact]
    public async Task CadastrarStartup_QuandoUsuarioJaPossuiPerfil_DeveRetornar409ENaoPersistirNovoPerfil()
    {
        await SemearAsync(PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build());

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Startup, DtoBuilder.CadastrarStartup());

        await resposta.DeveSerErroAsync(HttpStatusCode.Conflict, "O usuário já possui um perfil cadastrado.");
        (await ConsultarAsync(db => db.Perfil.CountAsync(p => p.UsuarioId == _usuarioId, Ct))).ShouldBe(1);
        (await ConsultarAsync(db => db.Startup.CountAsync(Ct))).ShouldBe(0);
    }

    [Theory]
    [InlineData("nome-vazio", "Perfil.Nome", CodigosErroConst.CampoObrigatorio)]
    [InlineData("imagem-perfil-invalida", "Perfil.ImagemPerfilUrl", CodigosErroConst.UrlInvalida)]
    [InlineData("latitude-fora", "Perfil.Coordenada.Latitude", CodigosErroConst.ValorEntre)]
    [InlineData("pitch-vazio", "Pitch", CodigosErroConst.CampoObrigatorio)]
    [InlineData("pitch-longo", "Pitch", CodigosErroConst.TamanhoMaximo)]
    [InlineData("data-futura", "DataFundacao", CodigosErroConst.DataFutura)]
    [InlineData("data-vazia", "DataFundacao", CodigosErroConst.CampoObrigatorio)]
    [InlineData("equipe-zero", "TamanhoEquipe", CodigosErroConst.ValorEntre)]
    [InlineData("equipe-acima-de-smallint", "TamanhoEquipe", CodigosErroConst.ValorEntre)]
    [InlineData("valor-negativo", "ValorBuscado", CodigosErroConst.ValorMinimo)]
    [InlineData("valor-tres-casas", "ValorBuscado", CodigosErroConst.PrecisaoIncorreta)]
    [InlineData("setor-invalido", "Setor", CodigosErroConst.ValorInvalidoEnum)]
    public async Task CadastrarStartup_ComPayloadInvalido_DeveRetornar400ENaoPersistir(string cenario, string campo, string codigo)
    {
        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Perfil.Startup, PayloadInvalido(cenario));

        await resposta.DeveSerErroDeValidacaoAsync(campo, codigo);
        (await ConsultarAsync(db => db.Perfil.CountAsync(Ct))).ShouldBe(0);
    }

    private static CadastrarPerfilStartupDTO PayloadInvalido(string cenario)
    {
        var valido = DtoBuilder.CadastrarStartup();
        var perfil = DtoBuilder.CadastrarPerfil();

        return cenario switch
        {
            "nome-vazio" => valido with { Perfil = perfil with { Nome = "" } },
            "imagem-perfil-invalida" => valido with { Perfil = perfil with { ImagemPerfilUrl = "nao-eh-url" } },
            "latitude-fora" => valido with { Perfil = perfil with { Coordenada = new CoordenadaDTO(-90.0001, -34.877) } },
            "pitch-vazio" => valido with { Pitch = "" },
            "pitch-longo" => valido with { Pitch = new string('a', TamanhosColunasConst.Startup.Pitch + 1) },
            "data-futura" => valido with { DataFundacao = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2) },
            "data-vazia" => valido with { DataFundacao = default },
            "equipe-zero" => valido with { TamanhoEquipe = 0 },
            "equipe-acima-de-smallint" => valido with { TamanhoEquipe = short.MaxValue + 1 },
            "valor-negativo" => valido with { ValorBuscado = -10m },
            "valor-tres-casas" => valido with { ValorBuscado = 1_000.555m },
            "setor-invalido" => valido with { Setor = "TECNOLOGIA" },
            _ => throw new ArgumentOutOfRangeException(nameof(cenario), cenario, null)
        };
    }
}
