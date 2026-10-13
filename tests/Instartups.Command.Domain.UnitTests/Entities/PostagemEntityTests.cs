using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Domain.UnitTests.Entities;

public class PostagemEntityTests
{
    private const string UrlMidia = "https://cdn.instartups.com/video.mp4";

    #region Criar

    [Fact]
    public void Criar_DeveCriarPostagemAtivaComDescricaoEAutor()
    {
        var autorId = Guid.NewGuid();

        var postagem = PostagemEntity.Criar("Nova postagem", autorId);

        postagem.Descricao.ShouldBe("Nova postagem");
        postagem.AutorId.ShouldBe(autorId);
        postagem.Ativo.ShouldBeTrue();
    }

    [Fact]
    public void Criar_DeveIniciarSemCurtidasESemMidias()
    {
        var postagem = PostagemEntity.Criar("Nova postagem", Guid.NewGuid());

        postagem.TotalCurtidas.Total.ShouldBe(0);
        postagem.MidiasPostagem.ShouldBeEmpty();
    }

    #endregion

    #region EditarDescricao

    [Fact]
    public void EditarDescricao_QuandoPostagemAtiva_DeveAtualizarDescricaoEData()
    {
        var postagem = PostagemBuilder.Nova().ComDescricao("Antiga").Build();

        postagem.EditarDescricao("Nova");

        postagem.Descricao.ShouldBe("Nova");
        postagem.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void EditarDescricao_QuandoPostagemDesativada_DeveLancarPostagemDesativadaException()
    {
        var postagem = PostagemBuilder.Nova().ComDescricao("Antiga").Desativada().Build();

        var acao = () => postagem.EditarDescricao("Nova");

        acao.ShouldThrow<PostagemDesativadaException>();
        postagem.Descricao.ShouldBe("Antiga");
    }

    #endregion

    #region AdicionarMidiaPostagem

    [Theory]
    [InlineData(TiposMidiaEnum.Foto)]
    [InlineData(TiposMidiaEnum.Video)]
    public void AdicionarMidiaPostagem_QuandoPostagemAtiva_DeveAdicionarMidiaVinculadaAPostagem(TiposMidiaEnum tipo)
    {
        var postagem = PostagemBuilder.Nova().Build();

        postagem.AdicionarMidiaPostagem(UrlMidia, tipo);

        var midia = postagem.MidiasPostagem.ShouldHaveSingleItem();
        midia.UrlMidia.ShouldBe(UrlMidia);
        midia.TipoMidia.ShouldBe(tipo);
        midia.PostagemId.ShouldBe(postagem.Id);
    }

    [Fact]
    public void AdicionarMidiaPostagem_QuandoPostagemTemQuatroMidias_DevePermitirAQuinta()
    {
        var postagem = PostagemBuilder.Nova().ComMidias(4).Build();

        postagem.AdicionarMidiaPostagem(UrlMidia, TiposMidiaEnum.Video);

        postagem.MidiasPostagem.Count.ShouldBe(5);
    }

    [Fact]
    public void AdicionarMidiaPostagem_QuandoPostagemJaTemCincoMidias_DeveLancarLimiteMidiasPostagemExcedidoException()
    {
        var postagem = PostagemBuilder.Nova().ComMidias(5).Build();

        var acao = () => postagem.AdicionarMidiaPostagem(UrlMidia, TiposMidiaEnum.Foto);

        acao.ShouldThrow<LimiteMidiasPostagemExcedidoException>()
            .Message.ShouldBe("Não é possível adicionar mais de 5 mídias a uma postagem.");
        postagem.MidiasPostagem.Count.ShouldBe(5);
    }

    [Fact]
    public void AdicionarMidiaPostagem_QuandoPostagemDesativada_DeveLancarPostagemDesativadaException()
    {
        var postagem = PostagemBuilder.Nova().Desativada().Build();

        var acao = () => postagem.AdicionarMidiaPostagem(UrlMidia, TiposMidiaEnum.Foto);

        acao.ShouldThrow<PostagemDesativadaException>();
        postagem.MidiasPostagem.ShouldBeEmpty();
    }

    [Fact]
    public void AdicionarMidiaPostagem_QuandoPostagemDesativadaComCincoMidias_DeveValidarStatusAntesDoLimite()
    {
        var postagem = PostagemBuilder.Nova().ComMidias(5).Desativada().Build();

        var acao = () => postagem.AdicionarMidiaPostagem(UrlMidia, TiposMidiaEnum.Foto);

        acao.ShouldThrow<PostagemDesativadaException>();
    }

    #endregion

    #region RemoverMidiaPostagem

    [Fact]
    public void RemoverMidiaPostagem_QuandoMidiaExiste_DeveRemoverApenasAMidiaInformada()
    {
        var postagem = PostagemBuilder.Nova().ComMidias(3).Build();
        var midiaRemovida = postagem.MidiasPostagem.ElementAt(1);

        postagem.RemoverMidiaPostagem(midiaRemovida.Id);

        postagem.MidiasPostagem.Count.ShouldBe(2);
        postagem.MidiasPostagem.ShouldNotContain(midiaRemovida);
    }

    [Fact]
    public void RemoverMidiaPostagem_QuandoMidiaNaoExiste_DeveLancarMidiaPostagemNaoEncontradaException()
    {
        var postagem = PostagemBuilder.Nova().ComMidias(2).Build();

        var acao = () => postagem.RemoverMidiaPostagem(Guid.NewGuid());

        acao.ShouldThrow<MidiaPostagemNaoEncontradaException>();
        postagem.MidiasPostagem.Count.ShouldBe(2);
    }

    [Fact]
    public void RemoverMidiaPostagem_QuandoPostagemDesativada_DeveLancarPostagemDesativadaException()
    {
        var postagem = PostagemBuilder.Nova().ComMidias(1).Desativada().Build();
        var midiaId = postagem.MidiasPostagem.Single().Id;

        var acao = () => postagem.RemoverMidiaPostagem(midiaId);

        acao.ShouldThrow<PostagemDesativadaException>();
        postagem.MidiasPostagem.Count.ShouldBe(1);
    }

    [Fact]
    public void RemoverMidiaPostagem_QuandoPostagemNoLimite_DeveLiberarVagaParaNovaMidia()
    {
        var postagem = PostagemBuilder.Nova().ComMidias(5).Build();

        postagem.RemoverMidiaPostagem(postagem.MidiasPostagem.First().Id);
        postagem.AdicionarMidiaPostagem(UrlMidia, TiposMidiaEnum.Video);

        postagem.MidiasPostagem.Count.ShouldBe(5);
        postagem.MidiasPostagem.ShouldContain(m => m.UrlMidia == UrlMidia);
    }

    #endregion

    #region MidiasPostagem

    [Fact]
    public void MidiasPostagem_DeveSerSomenteLeitura()
    {
        var postagem = PostagemBuilder.Nova().ComMidias(1).Build();

        var colecao = postagem.MidiasPostagem as ICollection<MidiaPostagemEntity>;

        colecao.ShouldNotBeNull();
        colecao.IsReadOnly.ShouldBeTrue();
        Should.Throw<NotSupportedException>(() => colecao.Clear());
    }

    #endregion

    #region Ativar e Desativar

    [Fact]
    public void Desativar_QuandoPostagemAtiva_DeveDesativarEAtualizarData()
    {
        var postagem = PostagemBuilder.Nova().Build();

        postagem.Desativar();

        postagem.Ativo.ShouldBeFalse();
        postagem.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void Desativar_QuandoPostagemJaDesativada_DeveLancarPostagemJaDesativadaException()
    {
        var postagem = PostagemBuilder.Nova().Desativada().Build();

        var acao = postagem.Desativar;

        acao.ShouldThrow<PostagemJaDesativadaException>();
    }

    [Fact]
    public void Ativar_QuandoPostagemDesativada_DeveAtivarEAtualizarData()
    {
        var postagem = PostagemBuilder.Nova().Desativada().Build();

        postagem.Ativar();

        postagem.Ativo.ShouldBeTrue();
        postagem.DataAtualizacao.ShouldNotBeNull();
    }

    [Fact]
    public void Ativar_QuandoPostagemJaAtiva_DeveLancarPostagemJaAtivaException()
    {
        var postagem = PostagemBuilder.Nova().Build();

        var acao = postagem.Ativar;

        acao.ShouldThrow<PostagemJaAtivaException>();
        postagem.DataAtualizacao.ShouldBeNull();
    }

    [Fact]
    public void Ativar_QuandoPostagemReativada_DevePermitirEditarNovamente()
    {
        var postagem = PostagemBuilder.Nova().Desativada().Build();

        postagem.Ativar();
        postagem.EditarDescricao("Editada após reativação");

        postagem.Descricao.ShouldBe("Editada após reativação");
    }

    #endregion
}
