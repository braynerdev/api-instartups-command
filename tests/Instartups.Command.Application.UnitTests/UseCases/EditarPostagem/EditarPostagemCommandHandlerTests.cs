using Instartups.Command.Application.UnitTests.Fixtures;
using Instartups.Command.Application.UseCases.EditarPostagem;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Tests.Common.Builders.Commands;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Application.UnitTests.UseCases.EditarPostagem;

public class EditarPostagemCommandHandlerTests
{
    private readonly UnitOfWorkMock _uow = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly PerfilEntity _autor;
    private readonly EditarPostagemCommandHandler _handler;

    public EditarPostagemCommandHandlerTests()
    {
        _autor = PerfilBuilder.Startup().ComUsuarioId(_userId).Build();
        _handler = new EditarPostagemCommandHandler(_uow.UnitOfWork);
    }

    private PostagemEntity ConfigurarPostagem(PostagemBuilder builder)
    {
        var postagem = builder.Build();
        _uow.Postagem.ObterPorIdComMidiasAsync(postagem.Id, Arg.Any<CancellationToken>()).Returns(postagem);
        return postagem;
    }

    private EditarPostagemCommand Command(PostagemEntity postagem) =>
        PostagemCommandBuilder.EditarPostagem(postagem.Id, _userId);

    [Fact]
    public async Task Handle_QuandoAutorEditaPostagemAtiva_DeveAtualizarDescricaoECommitar()
    {
        _uow.ComPerfilDoUsuario(_autor);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor).ComDescricao("Antiga"));
        var command = Command(postagem);

        await _handler.Handle(command, _ct);

        postagem.Descricao.ShouldBe(command.Descricao);
        await _uow.Postagem.Received(1).ObterPorIdComMidiasAsync(postagem.Id, _ct);
        await _uow.DeveTerCommitadoUmaVez(_ct);
    }

    [Fact]
    public async Task Handle_QuandoCommandTemMidiasRemovidas_DeveRemoverAsMidiasInformadas()
    {
        _uow.ComPerfilDoUsuario(_autor);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor).ComMidias(3));
        var removidas = postagem.MidiasPostagem.Take(2).Select(m => m.Id).ToList();
        var mantida = postagem.MidiasPostagem.Last();

        await _handler.Handle(Command(postagem) with { MidiasRemovidas = removidas }, _ct);

        postagem.MidiasPostagem.ShouldHaveSingleItem().ShouldBe(mantida);
    }

    [Fact]
    public async Task Handle_QuandoCommandTemMidiasAdicionadas_DeveAdicionarAsMidiasInformadas()
    {
        _uow.ComPerfilDoUsuario(_autor);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor).ComMidias(1));
        var novaMidia = PostagemCommandBuilder.Midia(7, TiposMidiaEnum.VIDEO);

        await _handler.Handle(Command(postagem) with { MidiasAdicionadas = [novaMidia] }, _ct);

        postagem.MidiasPostagem.Count.ShouldBe(2);
        postagem.MidiasPostagem.ShouldContain(m => m.UrlMidia == novaMidia.Url && m.TipoMidia == TiposMidiaEnum.VIDEO);
    }

    [Fact]
    public async Task Handle_QuandoPostagemNoLimiteDeMidias_DeveRemoverAntesDeAdicionar()
    {
        _uow.ComPerfilDoUsuario(_autor);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor).ComMidias(5));
        var command = Command(postagem) with
        {
            MidiasRemovidas = [postagem.MidiasPostagem.First().Id],
            MidiasAdicionadas = [PostagemCommandBuilder.Midia(9)]
        };

        await _handler.Handle(command, _ct);

        postagem.MidiasPostagem.Count.ShouldBe(5);
        postagem.MidiasPostagem.ShouldContain(m => m.UrlMidia == PostagemCommandBuilder.Midia(9).Url);
        await _uow.DeveTerCommitadoUmaVez(_ct);
    }

    [Fact]
    public async Task Handle_QuandoPerfilNaoEncontrado_DeveLancarPerfilNaoEncontradoExceptionSemBuscarPostagem()
    {
        _uow.SemPerfilDoUsuario(_userId);

        var acao = () => _handler.Handle(PostagemCommandBuilder.EditarPostagem(userId: _userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
        await _uow.Postagem.DidNotReceive().ObterPorIdComMidiasAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilDesativado_DeveLancarPerfilDesativadoExceptionSemBuscarPostagem()
    {
        _uow.ComPerfilDoUsuario(PerfilBuilder.Startup().ComUsuarioId(_userId).Desativado().Build());

        var acao = () => _handler.Handle(PostagemCommandBuilder.EditarPostagem(userId: _userId), _ct);

        await acao.ShouldThrowAsync<PerfilDesativadoException>();
        await _uow.Postagem.DidNotReceive().ObterPorIdComMidiasAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPostagemNaoEncontrada_DeveLancarPostagemNaoEncontradaException()
    {
        _uow.ComPerfilDoUsuario(_autor);
        var command = PostagemCommandBuilder.EditarPostagem(userId: _userId);
        _uow.Postagem.ObterPorIdComMidiasAsync(command.PostagemId, Arg.Any<CancellationToken>()).Returns((PostagemEntity?)null);

        var acao = () => _handler.Handle(command, _ct);

        await acao.ShouldThrowAsync<PostagemNaoEncontradaException>();
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilNaoEhAutor_DeveLancarPerfilNaoEhAutorPostagemException()
    {
        _uow.ComPerfilDoUsuario(_autor);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComDescricao("Original"));

        var acao = () => _handler.Handle(Command(postagem), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEhAutorPostagemException>();
        postagem.Descricao.ShouldBe("Original");
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPostagemDesativada_DeveLancarPostagemDesativadaException()
    {
        _uow.ComPerfilDoUsuario(_autor);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor).Desativada());

        var acao = () => _handler.Handle(Command(postagem), _ct);

        await acao.ShouldThrowAsync<PostagemDesativadaException>();
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoMidiaRemovidaNaoExiste_DeveLancarMidiaPostagemNaoEncontradaException()
    {
        _uow.ComPerfilDoUsuario(_autor);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor).ComMidias(1));

        var acao = () => _handler.Handle(Command(postagem) with { MidiasRemovidas = [Guid.NewGuid()] }, _ct);

        await acao.ShouldThrowAsync<MidiaPostagemNaoEncontradaException>();
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoMidiaRemovidaInformadaDuasVezes_DeveLancarMidiaPostagemNaoEncontradaException()
    {
        _uow.ComPerfilDoUsuario(_autor);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor).ComMidias(1));
        var midiaId = postagem.MidiasPostagem.Single().Id;

        var acao = () => _handler.Handle(Command(postagem) with { MidiasRemovidas = [midiaId, midiaId] }, _ct);

        await acao.ShouldThrowAsync<MidiaPostagemNaoEncontradaException>();
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoMidiasAdicionadasExcedemOLimite_DeveLancarLimiteMidiasPostagemExcedidoException()
    {
        _uow.ComPerfilDoUsuario(_autor);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor).ComMidias(4));

        var acao = () => _handler.Handle(Command(postagem) with { MidiasAdicionadas = PostagemCommandBuilder.Midias(2) }, _ct);

        await acao.ShouldThrowAsync<LimiteMidiasPostagemExcedidoException>();
        await _uow.NaoDeveTerCommitado();
    }
}
