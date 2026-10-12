using Instartups.Command.Application.UnitTests.Fixtures;
using Instartups.Command.Application.UseCases.ReativarPostagem;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Application.UnitTests.UseCases.ReativarPostagem;

public class ReativarPostagemCommandHandlerTests
{
    private readonly UnitOfWorkMock _uow = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly ReativarPostagemCommandHandler _handler;

    public ReativarPostagemCommandHandlerTests()
    {
        _handler = new ReativarPostagemCommandHandler(_uow.UnitOfWork);
    }

    private PostagemEntity ConfigurarPostagem(PostagemBuilder builder)
    {
        var postagem = builder.Build();
        _uow.Postagem.GetByIdAsync(postagem.Id, Arg.Any<CancellationToken>()).Returns(postagem);
        return postagem;
    }

    [Fact]
    public async Task Handle_QuandoAutorReativaPostagemDesativada_DeveReativarESomarCurtidasAoAutor()
    {
        var autor = PerfilBuilder.Startup().ComUsuarioId(_userId).ComTotalCurtidas(6).Build();
        _uow.ComPerfilDoUsuario(autor);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(4).Desativada());

        await _handler.Handle(new ReativarPostagemCommand(postagem.Id, _userId), _ct);

        postagem.Ativo.ShouldBeTrue();
        autor.TotalCurtidas.Total.ShouldBe(10);
        await _uow.Postagem.Received(1).GetByIdAsync(postagem.Id, _ct);
        await _uow.DeveTerCommitadoUmaVez(_ct);
    }

    [Fact]
    public async Task Handle_QuandoPerfilNaoEncontrado_DeveLancarPerfilNaoEncontradoExceptionSemBuscarPostagem()
    {
        _uow.SemPerfilDoUsuario(_userId);

        var acao = () => _handler.Handle(new ReativarPostagemCommand(Guid.NewGuid(), _userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
        await _uow.Postagem.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPostagemNaoEncontrada_DeveLancarPostagemNaoEncontradaException()
    {
        _uow.ComPerfilDoUsuario(PerfilBuilder.Startup().ComUsuarioId(_userId).Build());
        var postagemId = Guid.NewGuid();
        _uow.Postagem.GetByIdAsync(postagemId, Arg.Any<CancellationToken>()).Returns((PostagemEntity?)null);

        var acao = () => _handler.Handle(new ReativarPostagemCommand(postagemId, _userId), _ct);

        await acao.ShouldThrowAsync<PostagemNaoEncontradaException>();
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilNaoEhAutor_DeveLancarPerfilNaoEhAutorPostagemException()
    {
        _uow.ComPerfilDoUsuario(PerfilBuilder.Startup().ComUsuarioId(_userId).Build());
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().Desativada());

        var acao = () => _handler.Handle(new ReativarPostagemCommand(postagem.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEhAutorPostagemException>();
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoAutorDesativado_DeveLancarPerfilDesativadoException()
    {
        var autor = PerfilBuilder.Startup().ComUsuarioId(_userId).Desativado().Build();
        _uow.ComPerfilDoUsuario(autor);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(autor).Desativada());

        var acao = () => _handler.Handle(new ReativarPostagemCommand(postagem.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilDesativadoException>();
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPostagemJaAtiva_DeveLancarPostagemJaAtivaException()
    {
        var autor = PerfilBuilder.Startup().ComUsuarioId(_userId).Build();
        _uow.ComPerfilDoUsuario(autor);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(autor));

        var acao = () => _handler.Handle(new ReativarPostagemCommand(postagem.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PostagemJaAtivaException>();
        await _uow.NaoDeveTerCommitado();
    }
}
