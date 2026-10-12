using Instartups.Command.Application.UnitTests.Fixtures;
using Instartups.Command.Application.UseCases.CurtirPostagem;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Application.UnitTests.UseCases.CurtirPostagem;

public class CurtirPostagemCommandHandlerTests
{
    private readonly UnitOfWorkMock _uow = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly PerfilEntity _perfil;
    private readonly PerfilEntity _autor = PerfilBuilder.Startup().ComTotalCurtidas(10).Build();
    private readonly CurtirPostagemCommandHandler _handler;

    public CurtirPostagemCommandHandlerTests()
    {
        _perfil = PerfilBuilder.Investidor().ComUsuarioId(_userId).Build();
        _handler = new CurtirPostagemCommandHandler(_uow.UnitOfWork);
    }

    private PostagemEntity ConfigurarPostagem(PostagemBuilder builder)
    {
        var postagem = builder.Build();
        _uow.Postagem.ObterPorIdComAutorAsync(postagem.Id, Arg.Any<CancellationToken>()).Returns(postagem);
        return postagem;
    }

    private void ConfigurarJaCurtiu(PostagemEntity postagem, bool jaCurtiu) =>
        _uow.CurtidaPostagem.JaCurtiuAsync(postagem.Id, _perfil.Id, Arg.Any<CancellationToken>()).Returns(jaCurtiu);

    [Fact]
    public async Task Handle_QuandoPerfilAindaNaoCurtiu_DeveAdicionarCurtidaDoPerfilNaPostagem()
    {
        _uow.ComPerfilDoUsuario(_perfil);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor));
        ConfigurarJaCurtiu(postagem, false);

        await _handler.Handle(new CurtirPostagemCommand(postagem.Id, _userId), _ct);

        _uow.CurtidaPostagem.Received(1).Add(Arg.Is<CurtidaPostagemEntity>(c =>
            c.PostagemId == postagem.Id && c.PerfilAutorCurtidaId == _perfil.Id));
        await _uow.DeveTerCommitadoUmaVez(_ct);
    }

    [Fact]
    public async Task Handle_QuandoPerfilAindaNaoCurtiu_DeveIncrementarCurtidasDaPostagemEDoAutor()
    {
        _uow.ComPerfilDoUsuario(_perfil);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor).ComTotalCurtidas(2));
        ConfigurarJaCurtiu(postagem, false);

        await _handler.Handle(new CurtirPostagemCommand(postagem.Id, _userId), _ct);

        postagem.TotalCurtidas.Total.ShouldBe(3);
        _autor.TotalCurtidas.Total.ShouldBe(11);
    }

    [Fact]
    public async Task Handle_DeveVerificarCurtidaExistentePelosIdsDaPostagemEDoPerfil()
    {
        _uow.ComPerfilDoUsuario(_perfil);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor));

        await _handler.Handle(new CurtirPostagemCommand(postagem.Id, _userId), _ct);

        await _uow.CurtidaPostagem.Received(1).JaCurtiuAsync(postagem.Id, _perfil.Id, _ct);
    }

    [Fact]
    public async Task Handle_QuandoPerfilNaoEncontrado_DeveLancarPerfilNaoEncontradoExceptionSemBuscarPostagem()
    {
        _uow.SemPerfilDoUsuario(_userId);

        var acao = () => _handler.Handle(new CurtirPostagemCommand(Guid.NewGuid(), _userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
        await _uow.Postagem.DidNotReceive().ObterPorIdComAutorAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        _uow.CurtidaPostagem.DidNotReceive().Add(Arg.Any<CurtidaPostagemEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPostagemNaoEncontrada_DeveLancarPostagemNaoEncontradaExceptionSemVerificarCurtida()
    {
        _uow.ComPerfilDoUsuario(_perfil);
        var postagemId = Guid.NewGuid();
        _uow.Postagem.ObterPorIdComAutorAsync(postagemId, Arg.Any<CancellationToken>()).Returns((PostagemEntity?)null);

        var acao = () => _handler.Handle(new CurtirPostagemCommand(postagemId, _userId), _ct);

        await acao.ShouldThrowAsync<PostagemNaoEncontradaException>();
        await _uow.CurtidaPostagem.DidNotReceive().JaCurtiuAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilJaCurtiu_DeveLancarPostagemJaCurtidaException()
    {
        _uow.ComPerfilDoUsuario(_perfil);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor).ComTotalCurtidas(1));
        ConfigurarJaCurtiu(postagem, true);

        var acao = () => _handler.Handle(new CurtirPostagemCommand(postagem.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PostagemJaCurtidaException>();
        postagem.TotalCurtidas.Total.ShouldBe(1);
        _uow.CurtidaPostagem.DidNotReceive().Add(Arg.Any<CurtidaPostagemEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilCurteAPropriaPostagem_DeveLancarPerfilNaoPodeCurtirPropriaPostagemException()
    {
        _uow.ComPerfilDoUsuario(_perfil);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_perfil));

        var acao = () => _handler.Handle(new CurtirPostagemCommand(postagem.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoPodeCurtirPropriaPostagemException>();
        _uow.CurtidaPostagem.DidNotReceive().Add(Arg.Any<CurtidaPostagemEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPostagemDesativada_DeveLancarPostagemDesativadaException()
    {
        _uow.ComPerfilDoUsuario(_perfil);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor).Desativada());

        var acao = () => _handler.Handle(new CurtirPostagemCommand(postagem.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PostagemDesativadaException>();
        _uow.CurtidaPostagem.DidNotReceive().Add(Arg.Any<CurtidaPostagemEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilDesativado_DeveLancarPerfilDesativadoException()
    {
        var perfilDesativado = PerfilBuilder.Investidor().ComUsuarioId(_userId).Desativado().Build();
        _uow.ComPerfilDoUsuario(perfilDesativado);
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(_autor));

        var acao = () => _handler.Handle(new CurtirPostagemCommand(postagem.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilDesativadoException>();
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoAutorDaPostagemDesativado_DeveLancarPerfilDesativadoException()
    {
        _uow.ComPerfilDoUsuario(_perfil);
        var autorDesativado = PerfilBuilder.Startup().Desativado().Build();
        var postagem = ConfigurarPostagem(PostagemBuilder.Nova().ComAutor(autorDesativado));

        var acao = () => _handler.Handle(new CurtirPostagemCommand(postagem.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilDesativadoException>();
        await _uow.NaoDeveTerCommitado();
    }
}
