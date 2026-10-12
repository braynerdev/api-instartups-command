using Instartups.Command.Application.UnitTests.Fixtures;
using Instartups.Command.Application.UseCases.DeixarDeSeguirPerfil;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Application.UnitTests.UseCases.DeixarDeSeguirPerfil;

public class DeixarDeSeguirPerfilCommandHandlerTests
{
    private readonly UnitOfWorkMock _uow = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly DeixarDeSeguirPerfilCommandHandler _handler;

    public DeixarDeSeguirPerfilCommandHandlerTests()
    {
        _handler = new DeixarDeSeguirPerfilCommandHandler(_uow.UnitOfWork);
    }

    private PerfilSeguidorEntity ConfigurarRelacao(PerfilEntity seguidor, PerfilEntity seguido)
    {
        var perfilSeguidor = PerfilSeguidorBuilder.Novo().ComSeguidor(seguidor).ComSeguido(seguido).Build();
        _uow.PerfilSeguidor.ObterComSeguidoAsync(seguidor.Id, seguido.Id, Arg.Any<CancellationToken>()).Returns(perfilSeguidor);
        return perfilSeguidor;
    }

    [Fact]
    public async Task Handle_QuandoSegueOPerfil_DeveRemoverRelacaoECommitar()
    {
        var seguidor = PerfilBuilder.Investidor().ComUsuarioId(_userId).Build();
        var seguido = PerfilBuilder.Startup().Build();
        _uow.ComPerfilDoUsuario(seguidor);
        var perfilSeguidor = ConfigurarRelacao(seguidor, seguido);

        await _handler.Handle(new DeixarDeSeguirPerfilCommand(seguido.Id, _userId), _ct);

        _uow.PerfilSeguidor.Received(1).Remove(perfilSeguidor);
        await _uow.PerfilSeguidor.Received(1).ObterComSeguidoAsync(seguidor.Id, seguido.Id, _ct);
        await _uow.DeveTerCommitadoUmaVez(_ct);
    }

    [Fact]
    public async Task Handle_QuandoSegueOPerfil_DeveDecrementarSeguindoESeguidores()
    {
        var seguidor = PerfilBuilder.Investidor().ComUsuarioId(_userId).ComTotalSeguindo(3).Build();
        var seguido = PerfilBuilder.Startup().ComTotalSeguidores(6).Build();
        _uow.ComPerfilDoUsuario(seguidor);
        ConfigurarRelacao(seguidor, seguido);

        await _handler.Handle(new DeixarDeSeguirPerfilCommand(seguido.Id, _userId), _ct);

        seguidor.TotalSeguindo.Total.ShouldBe(2);
        seguido.TotalSeguidores.Total.ShouldBe(5);
    }

    [Fact]
    public async Task Handle_QuandoPerfilNaoEncontrado_DeveLancarPerfilNaoEncontradoExceptionSemBuscarRelacao()
    {
        _uow.SemPerfilDoUsuario(_userId);

        var acao = () => _handler.Handle(new DeixarDeSeguirPerfilCommand(Guid.NewGuid(), _userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
        await _uow.PerfilSeguidor.DidNotReceive().ObterComSeguidoAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoNaoSegueOPerfil_DeveLancarPerfilNaoSeguidoException()
    {
        var seguidor = PerfilBuilder.Investidor().ComUsuarioId(_userId).Build();
        _uow.ComPerfilDoUsuario(seguidor);
        var seguidoId = Guid.NewGuid();
        _uow.PerfilSeguidor.ObterComSeguidoAsync(seguidor.Id, seguidoId, Arg.Any<CancellationToken>()).Returns((PerfilSeguidorEntity?)null);

        var acao = () => _handler.Handle(new DeixarDeSeguirPerfilCommand(seguidoId, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoSeguidoException>();
        _uow.PerfilSeguidor.DidNotReceive().Remove(Arg.Any<PerfilSeguidorEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoSeguidoDesativado_DeveLancarPerfilDesativadoException()
    {
        var seguidor = PerfilBuilder.Investidor().ComUsuarioId(_userId).Build();
        var seguido = PerfilBuilder.Startup().Desativado().Build();
        _uow.ComPerfilDoUsuario(seguidor);
        ConfigurarRelacao(seguidor, seguido);

        var acao = () => _handler.Handle(new DeixarDeSeguirPerfilCommand(seguido.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilDesativadoException>();
        _uow.PerfilSeguidor.DidNotReceive().Remove(Arg.Any<PerfilSeguidorEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoSeguidorDesativado_DeveLancarPerfilDesativadoException()
    {
        var seguidor = PerfilBuilder.Investidor().ComUsuarioId(_userId).Desativado().Build();
        var seguido = PerfilBuilder.Startup().Build();
        _uow.ComPerfilDoUsuario(seguidor);
        ConfigurarRelacao(seguidor, seguido);

        var acao = () => _handler.Handle(new DeixarDeSeguirPerfilCommand(seguido.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilDesativadoException>();
        _uow.PerfilSeguidor.DidNotReceive().Remove(Arg.Any<PerfilSeguidorEntity>());
        await _uow.NaoDeveTerCommitado();
    }
}
