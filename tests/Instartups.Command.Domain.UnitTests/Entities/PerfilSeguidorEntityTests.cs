using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Domain.UnitTests.Entities;

public class PerfilSeguidorEntityTests
{
    [Fact]
    public void Criar_QuandoPerfisAtivosEDiferentes_DeveCriarRelacaoComSeguidorESeguido()
    {
        var seguidor = PerfilBuilder.Investidor().Build();
        var seguido = PerfilBuilder.Startup().Build();

        var perfilSeguidor = PerfilSeguidorEntity.Criar(seguidor, seguido);

        perfilSeguidor.Seguidor.ShouldBeSameAs(seguidor);
        perfilSeguidor.Seguido.ShouldBeSameAs(seguido);
        perfilSeguidor.Ativo.ShouldBeTrue();
    }

    [Fact]
    public void Criar_QuandoSeguidoDesativado_DeveLancarPerfilDesativadoException()
    {
        var seguidor = PerfilBuilder.Investidor().Build();
        var seguido = PerfilBuilder.Startup().Desativado().Build();

        var acao = () => PerfilSeguidorEntity.Criar(seguidor, seguido);

        acao.ShouldThrow<PerfilDesativadoException>();
    }

    [Fact]
    public void Criar_QuandoSeguidorDesativado_DeveLancarPerfilDesativadoException()
    {
        var seguidor = PerfilBuilder.Investidor().Desativado().Build();
        var seguido = PerfilBuilder.Startup().Build();

        var acao = () => PerfilSeguidorEntity.Criar(seguidor, seguido);

        acao.ShouldThrow<PerfilDesativadoException>();
    }

    [Fact]
    public void Criar_QuandoPerfilTentaSeguirASiMesmo_DeveLancarPerfilNaoPodeSeguirASiMesmoException()
    {
        var perfil = PerfilBuilder.Startup().Build();

        var acao = () => PerfilSeguidorEntity.Criar(perfil, perfil);

        acao.ShouldThrow<PerfilNaoPodeSeguirASiMesmoException>();
    }

    [Fact]
    public void Criar_QuandoPerfilDesativadoTentaSeguirASiMesmo_DeveValidarStatusAntesDoAutoSeguimento()
    {
        var perfil = PerfilBuilder.Startup().Desativado().Build();

        var acao = () => PerfilSeguidorEntity.Criar(perfil, perfil);

        acao.ShouldThrow<PerfilDesativadoException>();
    }
}
