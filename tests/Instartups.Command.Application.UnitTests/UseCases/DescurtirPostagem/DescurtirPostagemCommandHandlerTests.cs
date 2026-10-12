using Instartups.Command.Application.UnitTests.Fixtures;
using Instartups.Command.Application.UseCases.DescurtirPostagem;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Application.UnitTests.UseCases.DescurtirPostagem;

public class DescurtirPostagemCommandHandlerTests
{
    private readonly UnitOfWorkMock _uow = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly PerfilEntity _perfil;
    private readonly DescurtirPostagemCommandHandler _handler;

    public DescurtirPostagemCommandHandlerTests()
    {
        _perfil = PerfilBuilder.Investidor().ComUsuarioId(_userId).Build();
        _handler = new DescurtirPostagemCommandHandler(_uow.UnitOfWork);
    }

    private CurtidaPostagemEntity ConfigurarCurtida(PostagemEntity postagem)
    {
        var curtida = CurtidaPostagemBuilder.Nova().DaPostagem(postagem).DoPerfil(_perfil).Build();
        _uow.CurtidaPostagem.ObterComPostagemEAutorAsync(postagem.Id, _perfil.Id, Arg.Any<CancellationToken>()).Returns(curtida);
        return curtida;
    }

    [Fact]
    public async Task Handle_QuandoPerfilCurtiuPostagem_DeveRemoverCurtidaECommitar()
    {
        _uow.ComPerfilDoUsuario(_perfil);
        var postagem = PostagemBuilder.Nova().ComTotalCurtidas(1).Build();
        var curtida = ConfigurarCurtida(postagem);

        await _handler.Handle(new DescurtirPostagemCommand(postagem.Id, _userId), _ct);

        _uow.CurtidaPostagem.Received(1).Remove(curtida);
        await _uow.CurtidaPostagem.Received(1).ObterComPostagemEAutorAsync(postagem.Id, _perfil.Id, _ct);
        await _uow.DeveTerCommitadoUmaVez(_ct);
    }

    [Fact]
    public async Task Handle_QuandoPerfilCurtiuPostagem_DeveDecrementarCurtidasDaPostagemEDoAutor()
    {
        _uow.ComPerfilDoUsuario(_perfil);
        var autor = PerfilBuilder.Startup().ComTotalCurtidas(10).Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autor).ComTotalCurtidas(3).Build();
        ConfigurarCurtida(postagem);

        await _handler.Handle(new DescurtirPostagemCommand(postagem.Id, _userId), _ct);

        postagem.TotalCurtidas.Total.ShouldBe(2);
        autor.TotalCurtidas.Total.ShouldBe(9);
    }

    [Fact]
    public async Task Handle_QuandoPerfilNaoEncontrado_DeveLancarPerfilNaoEncontradoExceptionSemBuscarCurtida()
    {
        _uow.SemPerfilDoUsuario(_userId);

        var acao = () => _handler.Handle(new DescurtirPostagemCommand(Guid.NewGuid(), _userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
        await _uow.CurtidaPostagem.DidNotReceive().ObterComPostagemEAutorAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilNaoCurtiuPostagem_DeveLancarPostagemNaoCurtidaException()
    {
        _uow.ComPerfilDoUsuario(_perfil);
        var postagemId = Guid.NewGuid();
        _uow.CurtidaPostagem.ObterComPostagemEAutorAsync(postagemId, _perfil.Id, Arg.Any<CancellationToken>()).Returns((CurtidaPostagemEntity?)null);

        var acao = () => _handler.Handle(new DescurtirPostagemCommand(postagemId, _userId), _ct);

        await acao.ShouldThrowAsync<PostagemNaoCurtidaException>();
        _uow.CurtidaPostagem.DidNotReceive().Remove(Arg.Any<CurtidaPostagemEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPostagemDesativada_DeveLancarPostagemDesativadaException()
    {
        _uow.ComPerfilDoUsuario(_perfil);
        var postagem = PostagemBuilder.Nova().ComTotalCurtidas(1).Desativada().Build();
        ConfigurarCurtida(postagem);

        var acao = () => _handler.Handle(new DescurtirPostagemCommand(postagem.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PostagemDesativadaException>();
        postagem.TotalCurtidas.Total.ShouldBe(1);
        _uow.CurtidaPostagem.DidNotReceive().Remove(Arg.Any<CurtidaPostagemEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoAutorDaPostagemDesativado_DeveLancarPerfilDesativadoException()
    {
        _uow.ComPerfilDoUsuario(_perfil);
        var autorDesativado = PerfilBuilder.Startup().Desativado().Build();
        var postagem = PostagemBuilder.Nova().ComAutor(autorDesativado).ComTotalCurtidas(1).Build();
        ConfigurarCurtida(postagem);

        var acao = () => _handler.Handle(new DescurtirPostagemCommand(postagem.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilDesativadoException>();
        _uow.CurtidaPostagem.DidNotReceive().Remove(Arg.Any<CurtidaPostagemEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilDesativado_DeveLancarPerfilDesativadoException()
    {
        var perfilDesativado = PerfilBuilder.Investidor().ComUsuarioId(_userId).Desativado().Build();
        _uow.ComPerfilDoUsuario(perfilDesativado);
        var postagem = PostagemBuilder.Nova().ComTotalCurtidas(1).Build();
        var curtida = CurtidaPostagemBuilder.Nova().DaPostagem(postagem).DoPerfil(perfilDesativado).Build();
        _uow.CurtidaPostagem.ObterComPostagemEAutorAsync(postagem.Id, perfilDesativado.Id, Arg.Any<CancellationToken>()).Returns(curtida);

        var acao = () => _handler.Handle(new DescurtirPostagemCommand(postagem.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilDesativadoException>();
        _uow.CurtidaPostagem.DidNotReceive().Remove(Arg.Any<CurtidaPostagemEntity>());
        await _uow.NaoDeveTerCommitado();
    }
}
