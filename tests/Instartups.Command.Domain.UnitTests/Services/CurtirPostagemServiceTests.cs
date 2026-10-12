using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.Services;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Domain.UnitTests.Services;

public class CurtirPostagemServiceTests
{
    #region Curtir

    [Fact]
    public void Curtir_QuandoCenarioValido_DeveRetornarCurtidaComPostagemEPerfil()
    {
        var autor = PerfilBuilder.Startup().Build();
        var perfilQueCurtiu = PerfilBuilder.Investidor().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).Build();

        var curtida = CurtirPostagemService.Curtir(postagem, autor, perfilQueCurtiu, jaCurtiu: false);

        curtida.PostagemId.ShouldBe(postagem.Id);
        curtida.PerfilAutorCurtidaId.ShouldBe(perfilQueCurtiu.Id);
    }

    [Fact]
    public void Curtir_QuandoCenarioValido_DeveIncrementarCurtidasDaPostagemEDoAutor()
    {
        var autor = PerfilBuilder.Startup().ComTotalCurtidas(10).Build();
        var perfilQueCurtiu = PerfilBuilder.Investidor().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(2).Build();

        CurtirPostagemService.Curtir(postagem, autor, perfilQueCurtiu, jaCurtiu: false);

        postagem.TotalCurtidas.Total.ShouldBe(3);
        autor.TotalCurtidas.Total.ShouldBe(11);
        autor.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void Curtir_QuandoCenarioValido_NaoDeveAlterarCurtidasDoPerfilQueCurtiu()
    {
        var autor = PerfilBuilder.Startup().Build();
        var perfilQueCurtiu = PerfilBuilder.Investidor().ComTotalCurtidas(4).Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).Build();

        CurtirPostagemService.Curtir(postagem, autor, perfilQueCurtiu, jaCurtiu: false);

        perfilQueCurtiu.TotalCurtidas.Total.ShouldBe(4);
    }

    [Fact]
    public void Curtir_QuandoPerfilJaCurtiu_DeveLancarPostagemJaCurtidaException()
    {
        var autor = PerfilBuilder.Startup().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(1).Build();

        var acao = () => CurtirPostagemService.Curtir(postagem, autor, PerfilBuilder.Investidor().Build(), jaCurtiu: true);

        acao.ShouldThrow<PostagemJaCurtidaException>();
        postagem.TotalCurtidas.Total.ShouldBe(1);
        autor.TotalCurtidas.Total.ShouldBe(0);
    }

    [Fact]
    public void Curtir_QuandoPerfilJaCurtiuPostagemDesativada_DeveValidarCurtidaExistenteAntesDoStatus()
    {
        var autor = PerfilBuilder.Startup().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).Desativada().Build();

        var acao = () => CurtirPostagemService.Curtir(postagem, autor, PerfilBuilder.Investidor().Build(), jaCurtiu: true);

        acao.ShouldThrow<PostagemJaCurtidaException>();
    }

    [Fact]
    public void Curtir_QuandoAutorInformadoNaoEhAutorDaPostagem_DeveLancarArgumentException()
    {
        var postagem = PostagemBuilder.Nova().Build();
        var outroPerfil = PerfilBuilder.Startup().Build();

        var acao = () => CurtirPostagemService.Curtir(postagem, outroPerfil, PerfilBuilder.Investidor().Build(), jaCurtiu: false);

        var excecao = acao.ShouldThrow<ArgumentException>();
        excecao.ParamName.ShouldBe("autorPostagem");
        postagem.TotalCurtidas.Total.ShouldBe(0);
    }

    [Fact]
    public void Curtir_QuandoPostagemDesativada_DeveLancarPostagemDesativadaException()
    {
        var autor = PerfilBuilder.Startup().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).Desativada().Build();

        var acao = () => CurtirPostagemService.Curtir(postagem, autor, PerfilBuilder.Investidor().Build(), jaCurtiu: false);

        acao.ShouldThrow<PostagemDesativadaException>();
        postagem.TotalCurtidas.Total.ShouldBe(0);
    }

    [Fact]
    public void Curtir_QuandoAutorDaPostagemDesativado_DeveLancarPerfilDesativadoException()
    {
        var autor = PerfilBuilder.Startup().Desativado().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).Build();

        var acao = () => CurtirPostagemService.Curtir(postagem, autor, PerfilBuilder.Investidor().Build(), jaCurtiu: false);

        acao.ShouldThrow<PerfilDesativadoException>();
        postagem.TotalCurtidas.Total.ShouldBe(0);
        autor.TotalCurtidas.Total.ShouldBe(0);
    }

    [Fact]
    public void Curtir_QuandoPerfilQueCurtiuDesativado_DeveLancarPerfilDesativadoException()
    {
        var autor = PerfilBuilder.Startup().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).Build();
        var perfilQueCurtiu = PerfilBuilder.Investidor().Desativado().Build();

        var acao = () => CurtirPostagemService.Curtir(postagem, autor, perfilQueCurtiu, jaCurtiu: false);

        acao.ShouldThrow<PerfilDesativadoException>();
        postagem.TotalCurtidas.Total.ShouldBe(0);
        autor.TotalCurtidas.Total.ShouldBe(0);
    }

    [Fact]
    public void Curtir_QuandoPerfilCurteAPropriaPostagem_DeveLancarPerfilNaoPodeCurtirPropriaPostagemException()
    {
        var autor = PerfilBuilder.Startup().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).Build();

        var acao = () => CurtirPostagemService.Curtir(postagem, autor, autor, jaCurtiu: false);

        acao.ShouldThrow<PerfilNaoPodeCurtirPropriaPostagemException>();
        postagem.TotalCurtidas.Total.ShouldBe(0);
        autor.TotalCurtidas.Total.ShouldBe(0);
    }

    #endregion

    #region Descurtir

    [Fact]
    public void Descurtir_QuandoCenarioValido_DeveDecrementarCurtidasDaPostagemEDoAutor()
    {
        var autor = PerfilBuilder.Startup().ComTotalCurtidas(10).Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(3).Build();

        CurtirPostagemService.Descurtir(postagem, autor, PerfilBuilder.Investidor().Build());

        postagem.TotalCurtidas.Total.ShouldBe(2);
        autor.TotalCurtidas.Total.ShouldBe(9);
        autor.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void Descurtir_QuandoTotaisZerados_DeveManterTotaisEmZero()
    {
        var autor = PerfilBuilder.Startup().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).Build();

        CurtirPostagemService.Descurtir(postagem, autor, PerfilBuilder.Investidor().Build());

        postagem.TotalCurtidas.Total.ShouldBe(0);
        autor.TotalCurtidas.Total.ShouldBe(0);
    }

    [Fact]
    public void Descurtir_QuandoAutorInformadoNaoEhAutorDaPostagem_DeveLancarArgumentException()
    {
        var postagem = PostagemBuilder.Nova().ComTotalCurtidas(1).Build();

        var acao = () => CurtirPostagemService.Descurtir(postagem, PerfilBuilder.Startup().Build(), PerfilBuilder.Investidor().Build());

        acao.ShouldThrow<ArgumentException>().ParamName.ShouldBe("autorPostagem");
        postagem.TotalCurtidas.Total.ShouldBe(1);
    }

    [Fact]
    public void Descurtir_QuandoPostagemDesativada_DeveLancarPostagemDesativadaException()
    {
        var autor = PerfilBuilder.Startup().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(1).Desativada().Build();

        var acao = () => CurtirPostagemService.Descurtir(postagem, autor, PerfilBuilder.Investidor().Build());

        acao.ShouldThrow<PostagemDesativadaException>();
        postagem.TotalCurtidas.Total.ShouldBe(1);
    }

    [Fact]
    public void Descurtir_QuandoAutorDaPostagemDesativado_DeveLancarPerfilDesativadoException()
    {
        var autor = PerfilBuilder.Startup().ComTotalCurtidas(1).Desativado().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(1).Build();

        var acao = () => CurtirPostagemService.Descurtir(postagem, autor, PerfilBuilder.Investidor().Build());

        acao.ShouldThrow<PerfilDesativadoException>();
        postagem.TotalCurtidas.Total.ShouldBe(1);
        autor.TotalCurtidas.Total.ShouldBe(1);
    }

    [Fact]
    public void Descurtir_QuandoPerfilQueCurtiuDesativado_DeveLancarPerfilDesativadoException()
    {
        var autor = PerfilBuilder.Startup().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(1).Build();

        var acao = () => CurtirPostagemService.Descurtir(postagem, autor, PerfilBuilder.Investidor().Desativado().Build());

        acao.ShouldThrow<PerfilDesativadoException>();
        postagem.TotalCurtidas.Total.ShouldBe(1);
    }

    #endregion
}
