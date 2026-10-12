using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.Services;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Domain.UnitTests.Services;

public class ReativarPostagemServiceTests
{
    [Fact]
    public void Reativar_QuandoAutorAtivoEPostagemDesativada_DeveReativarPostagem()
    {
        var autor = PerfilBuilder.Startup().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).Desativada().Build();

        ReativarPostagemService.Reativar(postagem, autor);

        postagem.Ativo.ShouldBeTrue();
    }

    [Fact]
    public void Reativar_QuandoPostagemTemCurtidas_DeveSomarCurtidasDaPostagemAoTotalDoAutor()
    {
        var autor = PerfilBuilder.Startup().ComTotalCurtidas(6).Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(4).Desativada().Build();

        ReativarPostagemService.Reativar(postagem, autor);

        autor.TotalCurtidas.Total.ShouldBe(10);
        postagem.TotalCurtidas.Total.ShouldBe(4);
        autor.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void Reativar_AposDesativar_DeveRestaurarTotalDeCurtidasDoAutor()
    {
        var autor = PerfilBuilder.Startup().ComTotalCurtidas(10).Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(4).Build();

        DesativarPostagemService.Desativar(postagem, autor);
        ReativarPostagemService.Reativar(postagem, autor);

        autor.TotalCurtidas.Total.ShouldBe(10);
        postagem.Ativo.ShouldBeTrue();
    }

    [Fact]
    public void Reativar_QuandoAutorDesativado_DeveLancarPerfilDesativadoException()
    {
        var autor = PerfilBuilder.Startup().Desativado().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(4).Desativada().Build();

        var acao = () => ReativarPostagemService.Reativar(postagem, autor);

        acao.ShouldThrow<PerfilDesativadoException>();
        postagem.Ativo.ShouldBeFalse();
        autor.TotalCurtidas.Total.ShouldBe(0);
    }

    [Fact]
    public void Reativar_QuandoPerfilNaoEhAutor_DeveLancarPerfilNaoEhAutorPostagemException()
    {
        var outroPerfil = PerfilBuilder.Startup().Build();
        var postagem = PostagemBuilder.Nova().ComTotalCurtidas(4).Desativada().Build();

        var acao = () => ReativarPostagemService.Reativar(postagem, outroPerfil);

        acao.ShouldThrow<PerfilNaoEhAutorPostagemException>();
        postagem.Ativo.ShouldBeFalse();
        outroPerfil.TotalCurtidas.Total.ShouldBe(0);
    }

    [Fact]
    public void Reativar_QuandoPostagemJaAtiva_DeveLancarPostagemJaAtivaException()
    {
        var autor = PerfilBuilder.Startup().ComTotalCurtidas(2).Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(4).Build();

        var acao = () => ReativarPostagemService.Reativar(postagem, autor);

        acao.ShouldThrow<PostagemJaAtivaException>();
        autor.TotalCurtidas.Total.ShouldBe(2);
    }
}
