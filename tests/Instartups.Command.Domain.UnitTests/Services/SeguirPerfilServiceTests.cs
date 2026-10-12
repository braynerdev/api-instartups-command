using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.Services;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Domain.UnitTests.Services;

public class SeguirPerfilServiceTests
{
    #region Seguir

    [Fact]
    public void Seguir_QuandoCenarioValido_DeveRetornarRelacaoEntreSeguidorESeguido()
    {
        var seguidor = PerfilBuilder.Investidor().Build();
        var seguido = PerfilBuilder.Startup().Build();

        var perfilSeguidor = SeguirPerfilService.Seguir(seguidor, seguido, jaSegue: false);

        perfilSeguidor.Seguidor.ShouldBeSameAs(seguidor);
        perfilSeguidor.Seguido.ShouldBeSameAs(seguido);
    }

    [Fact]
    public void Seguir_QuandoCenarioValido_DeveIncrementarSeguindoDoSeguidorESeguidoresDoSeguido()
    {
        var seguidor = PerfilBuilder.Investidor().ComTotalSeguindo(2).ComTotalSeguidores(7).Build();
        var seguido = PerfilBuilder.Startup().ComTotalSeguidores(5).ComTotalSeguindo(3).Build();

        SeguirPerfilService.Seguir(seguidor, seguido, jaSegue: false);

        seguidor.TotalSeguindo.Total.ShouldBe(3);
        seguidor.TotalSeguidores.Total.ShouldBe(7);
        seguido.TotalSeguidores.Total.ShouldBe(6);
        seguido.TotalSeguindo.Total.ShouldBe(3);
        seguidor.DataAtualizacao.ShouldNotBeNull();
        seguido.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void Seguir_QuandoJaSegue_DeveLancarPerfilJaSeguidoException()
    {
        var seguidor = PerfilBuilder.Investidor().Build();
        var seguido = PerfilBuilder.Startup().Build();

        var acao = () => SeguirPerfilService.Seguir(seguidor, seguido, jaSegue: true);

        acao.ShouldThrow<PerfilJaSeguidoException>();
        seguidor.TotalSeguindo.Total.ShouldBe(0);
        seguido.TotalSeguidores.Total.ShouldBe(0);
    }

    [Fact]
    public void Seguir_QuandoJaSegueEPerfilDesativado_DeveValidarSeguimentoExistenteAntesDoStatus()
    {
        var seguidor = PerfilBuilder.Investidor().Desativado().Build();
        var seguido = PerfilBuilder.Startup().Build();

        var acao = () => SeguirPerfilService.Seguir(seguidor, seguido, jaSegue: true);

        acao.ShouldThrow<PerfilJaSeguidoException>();
    }

    [Fact]
    public void Seguir_QuandoSeguidorDesativado_DeveLancarPerfilDesativadoException()
    {
        var seguidor = PerfilBuilder.Investidor().Desativado().Build();
        var seguido = PerfilBuilder.Startup().Build();

        var acao = () => SeguirPerfilService.Seguir(seguidor, seguido, jaSegue: false);

        acao.ShouldThrow<PerfilDesativadoException>();
        seguido.TotalSeguidores.Total.ShouldBe(0);
    }

    [Fact]
    public void Seguir_QuandoSeguidoDesativado_DeveLancarPerfilDesativadoException()
    {
        var seguidor = PerfilBuilder.Investidor().Build();
        var seguido = PerfilBuilder.Startup().Desativado().Build();

        var acao = () => SeguirPerfilService.Seguir(seguidor, seguido, jaSegue: false);

        acao.ShouldThrow<PerfilDesativadoException>();
        seguidor.TotalSeguindo.Total.ShouldBe(0);
    }

    [Fact]
    public void Seguir_QuandoPerfilTentaSeguirASiMesmo_DeveLancarPerfilNaoPodeSeguirASiMesmoException()
    {
        var perfil = PerfilBuilder.Startup().Build();

        var acao = () => SeguirPerfilService.Seguir(perfil, perfil, jaSegue: false);

        acao.ShouldThrow<PerfilNaoPodeSeguirASiMesmoException>();
        perfil.TotalSeguindo.Total.ShouldBe(0);
        perfil.TotalSeguidores.Total.ShouldBe(0);
    }

    #endregion

    #region DeixarDeSeguir

    [Fact]
    public void DeixarDeSeguir_QuandoCenarioValido_DeveDecrementarSeguindoDoSeguidorESeguidoresDoSeguido()
    {
        var seguidor = PerfilBuilder.Investidor().ComTotalSeguindo(3).Build();
        var seguido = PerfilBuilder.Startup().ComTotalSeguidores(6).Build();

        SeguirPerfilService.DeixarDeSeguir(seguidor, seguido);

        seguidor.TotalSeguindo.Total.ShouldBe(2);
        seguido.TotalSeguidores.Total.ShouldBe(5);
        seguidor.DataAtualizacao.ShouldNotBeNull();
        seguido.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void DeixarDeSeguir_QuandoTotaisZerados_DeveManterTotaisEmZero()
    {
        var seguidor = PerfilBuilder.Investidor().Build();
        var seguido = PerfilBuilder.Startup().Build();

        SeguirPerfilService.DeixarDeSeguir(seguidor, seguido);

        seguidor.TotalSeguindo.Total.ShouldBe(0);
        seguido.TotalSeguidores.Total.ShouldBe(0);
    }

    [Fact]
    public void DeixarDeSeguir_QuandoSeguidorDesativado_DeveLancarPerfilDesativadoException()
    {
        var seguidor = PerfilBuilder.Investidor().ComTotalSeguindo(1).Desativado().Build();
        var seguido = PerfilBuilder.Startup().ComTotalSeguidores(1).Build();

        var acao = () => SeguirPerfilService.DeixarDeSeguir(seguidor, seguido);

        acao.ShouldThrow<PerfilDesativadoException>();
        seguidor.TotalSeguindo.Total.ShouldBe(1);
        seguido.TotalSeguidores.Total.ShouldBe(1);
    }

    [Fact]
    public void DeixarDeSeguir_QuandoSeguidoDesativado_DeveLancarPerfilDesativadoException()
    {
        var seguidor = PerfilBuilder.Investidor().ComTotalSeguindo(1).Build();
        var seguido = PerfilBuilder.Startup().ComTotalSeguidores(1).Desativado().Build();

        var acao = () => SeguirPerfilService.DeixarDeSeguir(seguidor, seguido);

        acao.ShouldThrow<PerfilDesativadoException>();
        seguidor.TotalSeguindo.Total.ShouldBe(1);
        seguido.TotalSeguidores.Total.ShouldBe(1);
    }

    #endregion
}
