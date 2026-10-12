using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.Services;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Domain.UnitTests.Services;

public class DesativarPostagemServiceTests
{
    [Fact]
    public void Desativar_QuandoAutorAtivoEPostagemAtiva_DeveDesativarPostagem()
    {
        var autor = PerfilBuilder.Startup().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).Build();

        DesativarPostagemService.Desativar(postagem, autor);

        postagem.Ativo.ShouldBeFalse();
    }

    [Fact]
    public void Desativar_QuandoPostagemTemCurtidas_DeveSubtrairCurtidasDaPostagemDoTotalDoAutor()
    {
        var autor = PerfilBuilder.Startup().ComTotalCurtidas(10).Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(4).Build();

        DesativarPostagemService.Desativar(postagem, autor);

        autor.TotalCurtidas.Total.ShouldBe(6);
        postagem.TotalCurtidas.Total.ShouldBe(4);
        autor.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void Desativar_QuandoCurtidasDaPostagemMaioresQueAsDoAutor_DeveZerarTotalDoAutor()
    {
        var autor = PerfilBuilder.Startup().ComTotalCurtidas(2).Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(5).Build();

        DesativarPostagemService.Desativar(postagem, autor);

        autor.TotalCurtidas.Total.ShouldBe(0);
    }

    [Fact]
    public void Desativar_QuandoAutorDesativado_DeveLancarPerfilDesativadoException()
    {
        var autor = PerfilBuilder.Startup().ComTotalCurtidas(5).Desativado().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(5).Build();

        var acao = () => DesativarPostagemService.Desativar(postagem, autor);

        acao.ShouldThrow<PerfilDesativadoException>();
        postagem.Ativo.ShouldBeTrue();
        autor.TotalCurtidas.Total.ShouldBe(5);
    }

    [Fact]
    public void Desativar_QuandoPerfilNaoEhAutor_DeveLancarPerfilNaoEhAutorPostagemException()
    {
        var outroPerfil = PerfilBuilder.Startup().ComTotalCurtidas(5).Build();
        var postagem = PostagemBuilder.Nova().ComTotalCurtidas(5).Build();

        var acao = () => DesativarPostagemService.Desativar(postagem, outroPerfil);

        acao.ShouldThrow<PerfilNaoEhAutorPostagemException>();
        postagem.Ativo.ShouldBeTrue();
        outroPerfil.TotalCurtidas.Total.ShouldBe(5);
    }

    [Fact]
    public void Desativar_QuandoPerfilDesativadoNaoEhAutor_DeveValidarStatusDoPerfilAntesDaAutoria()
    {
        var outroPerfil = PerfilBuilder.Startup().Desativado().Build();
        var postagem = PostagemBuilder.Nova().Build();

        var acao = () => DesativarPostagemService.Desativar(postagem, outroPerfil);

        acao.ShouldThrow<PerfilDesativadoException>();
    }

    [Fact]
    public void Desativar_QuandoPostagemJaDesativada_DeveLancarPostagemJaDesativadaException()
    {
        var autor = PerfilBuilder.Startup().ComTotalCurtidas(5).Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(3).Desativada().Build();

        var acao = () => DesativarPostagemService.Desativar(postagem, autor);

        acao.ShouldThrow<PostagemJaDesativadaException>();
        autor.TotalCurtidas.Total.ShouldBe(5);
    }
}
