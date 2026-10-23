using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.ValueObjects;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Domain.UnitTests.Entities;

public class PerfilEntityTests
{
    private static readonly Guid UsuarioId = Guid.NewGuid();

    private static PerfilEntity CriarStartupValida(string nome = "Instartups Tech", string? imagemPerfilUrl = "https://cdn.instartups.com/perfil.png", string? imagemFundoUrl = "https://cdn.instartups.com/fundo.png") =>
        PerfilEntity.CriarStartup(nome, -23.5505, -46.6333, UsuarioId, imagemPerfilUrl, imagemFundoUrl, "Pitch da startup", new DateOnly(2020, 1, 15), 10, 500_000m, SetorStartupEnum.Financas);

    private static PerfilEntity CriarInvestidorValido(string nome = "Fundo Investe") =>
        PerfilEntity.CriarInvestidor(nome, -22.9068, -43.1729, UsuarioId, "https://cdn.instartups.com/perfil.png", null, "Tese de investimento", 50_000m, 500_000m);

    private static void EditarStartup(PerfilEntity perfil) =>
        perfil.EditarStartup("  Nova Startup  ", 10.5, 20.5, "https://cdn.instartups.com/nova.png", null, "Novo pitch", new DateOnly(2018, 3, 10), 42, 2_000_000m, SetorStartupEnum.Varejo);

    private static void EditarInvestidor(PerfilEntity perfil) =>
        perfil.EditarInvestidor("  Novo Fundo  ", 10.5, 20.5, null, "https://cdn.instartups.com/fundo-novo.png", "Nova tese", 100_000m, 900_000m);

    #region CriarStartup

    [Fact]
    public void CriarStartup_DeveCriarPerfilDoTipoStartupComDadosInformados()
    {
        var perfil = CriarStartupValida();

        perfil.TipoPerfil.ShouldBe(TiposPerfisEnum.Startup);
        perfil.Nome.ShouldBe(NomeVO.Create("Instartups Tech"));
        perfil.Coordenada.ShouldBe(CoordenadaVO.Create(-23.5505, -46.6333));
        perfil.UsuarioId.ShouldBe(UsuarioId);
        perfil.ImagemPerfilUrl.ShouldBe("https://cdn.instartups.com/perfil.png");
        perfil.ImagemFundoUrl.ShouldBe("https://cdn.instartups.com/fundo.png");
        perfil.Ativo.ShouldBeTrue();
        perfil.DataAtualizacao.ShouldBeNull();
    }

    [Fact]
    public void CriarStartup_DeveIniciarTotaisZerados()
    {
        var perfil = CriarStartupValida();

        perfil.TotalCurtidas.ShouldBe(TotalVO.Zero());
        perfil.TotalSeguidores.ShouldBe(TotalVO.Zero());
        perfil.TotalSeguindo.ShouldBe(TotalVO.Zero());
    }

    [Fact]
    public void CriarStartup_DeveCriarStartupVinculadaAoPerfil()
    {
        var perfil = CriarStartupValida();

        perfil.Startup.ShouldNotBeNull();
        perfil.Startup.PerfilId.ShouldBe(perfil.Id);
        perfil.Startup.Pitch.ShouldBe("Pitch da startup");
        perfil.Startup.DataFundacao.ShouldBe(new DateOnly(2020, 1, 15));
        perfil.Startup.TamanhoEquipe.ShouldBe(10);
        perfil.Startup.ValorBuscado.ShouldBe(500_000m);
        perfil.Startup.Setor.ShouldBe(SetorStartupEnum.Financas);
        perfil.Investidor.ShouldBeNull();
    }

    [Fact]
    public void CriarStartup_QuandoNomeTemEspacosNasPontas_DeveRemoverOsEspacos()
    {
        var perfil = CriarStartupValida(nome: "  Instartups Tech  ");

        perfil.Nome.Nome.ShouldBe("Instartups Tech");
    }

    [Fact]
    public void CriarStartup_QuandoImagensNulas_DeveCriarSemImagens()
    {
        var perfil = CriarStartupValida(imagemPerfilUrl: null, imagemFundoUrl: null);

        perfil.ImagemPerfilUrl.ShouldBeNull();
        perfil.ImagemFundoUrl.ShouldBeNull();
    }

    #endregion

    #region CriarInvestidor

    [Fact]
    public void CriarInvestidor_DeveCriarPerfilDoTipoInvestidorComDadosInformados()
    {
        var perfil = CriarInvestidorValido();

        perfil.TipoPerfil.ShouldBe(TiposPerfisEnum.Investidor);
        perfil.Nome.Nome.ShouldBe("Fundo Investe");
        perfil.Coordenada.ShouldBe(CoordenadaVO.Create(-22.9068, -43.1729));
        perfil.UsuarioId.ShouldBe(UsuarioId);
        perfil.ImagemPerfilUrl.ShouldBe("https://cdn.instartups.com/perfil.png");
        perfil.ImagemFundoUrl.ShouldBeNull();
        perfil.Ativo.ShouldBeTrue();
        perfil.TotalCurtidas.Total.ShouldBe(0);
        perfil.TotalSeguidores.Total.ShouldBe(0);
        perfil.TotalSeguindo.Total.ShouldBe(0);
    }

    [Fact]
    public void CriarInvestidor_DeveCriarInvestidorVinculadoAoPerfil()
    {
        var perfil = CriarInvestidorValido();

        perfil.Investidor.ShouldNotBeNull();
        perfil.Investidor.PerfilId.ShouldBe(perfil.Id);
        perfil.Investidor.TeseInvestimento.ShouldBe("Tese de investimento");
        perfil.Investidor.TicketMinimo.ShouldBe(50_000m);
        perfil.Investidor.TicketMaximo.ShouldBe(500_000m);
        perfil.Startup.ShouldBeNull();
    }

    [Fact]
    public void CriarInvestidor_QuandoNomeTemEspacosNasPontas_DeveRemoverOsEspacos()
    {
        var perfil = CriarInvestidorValido(nome: " Fundo Investe ");

        perfil.Nome.Nome.ShouldBe("Fundo Investe");
    }

    #endregion

    #region EditarStartup

    [Fact]
    public void EditarStartup_QuandoPerfilStartupAtivo_DeveAtualizarDadosDoPerfil()
    {
        var perfil = CriarStartupValida();

        EditarStartup(perfil);

        perfil.Nome.Nome.ShouldBe("Nova Startup");
        perfil.Coordenada.ShouldBe(CoordenadaVO.Create(10.5, 20.5));
        perfil.ImagemPerfilUrl.ShouldBe("https://cdn.instartups.com/nova.png");
        perfil.ImagemFundoUrl.ShouldBeNull();
        perfil.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void EditarStartup_QuandoPerfilStartupAtivo_DeveAtualizarDadosDaStartup()
    {
        var perfil = CriarStartupValida();

        EditarStartup(perfil);

        perfil.Startup!.Pitch.ShouldBe("Novo pitch");
        perfil.Startup.DataFundacao.ShouldBe(new DateOnly(2018, 3, 10));
        perfil.Startup.TamanhoEquipe.ShouldBe(42);
        perfil.Startup.ValorBuscado.ShouldBe(2_000_000m);
        perfil.Startup.Setor.ShouldBe(SetorStartupEnum.Varejo);
        perfil.Startup.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void EditarStartup_DeveRetornarOProprioPerfil()
    {
        var perfil = CriarStartupValida();

        var resultado = perfil.EditarStartup("Nome", 1, 1, null, null, "Pitch", new DateOnly(2020, 1, 1), 1, 1m, SetorStartupEnum.Saude);

        resultado.ShouldBeSameAs(perfil);
    }

    [Fact]
    public void EditarStartup_DeveManterTipoUsuarioETotais()
    {
        var perfil = PerfilBuilder.Startup().ComUsuarioId(UsuarioId).ComTotalCurtidas(3).ComTotalSeguidores(2).ComTotalSeguindo(1).Build();

        EditarStartup(perfil);

        perfil.TipoPerfil.ShouldBe(TiposPerfisEnum.Startup);
        perfil.UsuarioId.ShouldBe(UsuarioId);
        perfil.TotalCurtidas.Total.ShouldBe(3);
        perfil.TotalSeguidores.Total.ShouldBe(2);
        perfil.TotalSeguindo.Total.ShouldBe(1);
    }

    [Fact]
    public void EditarStartup_QuandoPerfilEhInvestidor_DeveLancarPerfilNaoEhStartupException()
    {
        var perfil = CriarInvestidorValido();

        var acao = () => EditarStartup(perfil);

        acao.ShouldThrow<PerfilNaoEhStartupException>();
        perfil.Nome.Nome.ShouldBe("Fundo Investe");
        perfil.DataAtualizacao.ShouldBeNull();
    }

    [Fact]
    public void EditarStartup_QuandoPerfilDesativado_DeveLancarPerfilDesativadoException()
    {
        var perfil = PerfilBuilder.Startup().ComNome("Original").Desativado().Build();

        var acao = () => EditarStartup(perfil);

        acao.ShouldThrow<PerfilDesativadoException>();
        perfil.Nome.Nome.ShouldBe("Original");
    }

    [Fact]
    public void EditarStartup_QuandoPerfilInvestidorDesativado_DeveValidarTipoAntesDoStatus()
    {
        var perfil = PerfilBuilder.Investidor().Desativado().Build();

        var acao = () => EditarStartup(perfil);

        acao.ShouldThrow<PerfilNaoEhStartupException>();
    }

    #endregion

    #region EditarInvestidor

    [Fact]
    public void EditarInvestidor_QuandoPerfilInvestidorAtivo_DeveAtualizarDadosDoPerfil()
    {
        var perfil = CriarInvestidorValido();

        EditarInvestidor(perfil);

        perfil.Nome.Nome.ShouldBe("Novo Fundo");
        perfil.Coordenada.ShouldBe(CoordenadaVO.Create(10.5, 20.5));
        perfil.ImagemPerfilUrl.ShouldBeNull();
        perfil.ImagemFundoUrl.ShouldBe("https://cdn.instartups.com/fundo-novo.png");
        perfil.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void EditarInvestidor_QuandoPerfilInvestidorAtivo_DeveAtualizarDadosDoInvestidor()
    {
        var perfil = CriarInvestidorValido();

        EditarInvestidor(perfil);

        perfil.Investidor!.TeseInvestimento.ShouldBe("Nova tese");
        perfil.Investidor.TicketMinimo.ShouldBe(100_000m);
        perfil.Investidor.TicketMaximo.ShouldBe(900_000m);
        perfil.Investidor.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void EditarInvestidor_DeveRetornarOProprioPerfil()
    {
        var perfil = CriarInvestidorValido();

        var resultado = perfil.EditarInvestidor("Nome", 1, 1, null, null, "Tese", 1m, 2m);

        resultado.ShouldBeSameAs(perfil);
    }

    [Fact]
    public void EditarInvestidor_QuandoPerfilEhStartup_DeveLancarPerfilNaoEhInvestidorException()
    {
        var perfil = CriarStartupValida();

        var acao = () => EditarInvestidor(perfil);

        acao.ShouldThrow<PerfilNaoEhInvestidorException>();
        perfil.Nome.Nome.ShouldBe("Instartups Tech");
        perfil.DataAtualizacao.ShouldBeNull();
    }

    [Fact]
    public void EditarInvestidor_QuandoPerfilDesativado_DeveLancarPerfilDesativadoException()
    {
        var perfil = PerfilBuilder.Investidor().ComNome("Original").Desativado().Build();

        var acao = () => EditarInvestidor(perfil);

        acao.ShouldThrow<PerfilDesativadoException>();
        perfil.Nome.Nome.ShouldBe("Original");
    }

    [Fact]
    public void EditarInvestidor_QuandoPerfilStartupDesativado_DeveValidarTipoAntesDoStatus()
    {
        var perfil = PerfilBuilder.Startup().Desativado().Build();

        var acao = () => EditarInvestidor(perfil);

        acao.ShouldThrow<PerfilNaoEhInvestidorException>();
    }

    #endregion

    #region EditarCoordenadas

    [Fact]
    public void EditarCoordenadas_QuandoPerfilAtivo_DeveAtualizarCoordenadaEDataAtualizacao()
    {
        var perfil = CriarStartupValida();

        var resultado = perfil.EditarCoordenadas(-15.7939, -47.8828);

        resultado.ShouldBeSameAs(perfil);
        perfil.Coordenada.ShouldBe(CoordenadaVO.Create(-15.7939, -47.8828));
        perfil.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void EditarCoordenadas_QuandoPerfilDesativado_DeveLancarPerfilDesativadoException()
    {
        var perfil = PerfilBuilder.Startup().ComCoordenada(1, 2).Desativado().Build();

        var acao = () => perfil.EditarCoordenadas(3, 4);

        acao.ShouldThrow<PerfilDesativadoException>();
        perfil.Coordenada.ShouldBe(CoordenadaVO.Create(1, 2));
    }

    #endregion

    #region Ativar e Desativar

    [Fact]
    public void Desativar_QuandoPerfilAtivo_DeveDesativarEAtualizarData()
    {
        var perfil = CriarStartupValida();

        perfil.Desativar();

        perfil.Ativo.ShouldBeFalse();
        perfil.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void Desativar_QuandoPerfilJaDesativado_DeveLancarPerfilJaDesativadoException()
    {
        var perfil = PerfilBuilder.Startup().Desativado().Build();

        var acao = perfil.Desativar;

        acao.ShouldThrow<PerfilJaDesativadoException>();
        perfil.Ativo.ShouldBeFalse();
    }

    [Fact]
    public void Ativar_QuandoPerfilDesativado_DeveAtivarEAtualizarData()
    {
        var perfil = PerfilBuilder.Startup().Desativado().Build();
        var dataDesativacao = perfil.DataAtualizacao;

        perfil.Ativar();

        perfil.Ativo.ShouldBeTrue();
        perfil.DataAtualizacao.ShouldNotBeNull();
        perfil.DataAtualizacao.Value.ShouldBeGreaterThanOrEqualTo(dataDesativacao!.Value);
    }

    [Fact]
    public void Ativar_QuandoPerfilJaAtivo_DeveLancarPerfilJaAtivoException()
    {
        var perfil = CriarStartupValida();

        var acao = perfil.Ativar;

        acao.ShouldThrow<PerfilJaAtivoException>();
        perfil.DataAtualizacao.ShouldBeNull();
    }

    #endregion
}
