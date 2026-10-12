using Instartups.Command.Application.UnitTests.Fixtures;
using Instartups.Command.Application.UseCases.SeguirPerfil;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Application.UnitTests.UseCases.SeguirPerfil;

public class SeguirPerfilCommandHandlerTests
{
    private readonly UnitOfWorkMock _uow = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly SeguirPerfilCommandHandler _handler;

    public SeguirPerfilCommandHandlerTests()
    {
        _handler = new SeguirPerfilCommandHandler(_uow.UnitOfWork);
    }

    private PerfilEntity ConfigurarSeguidor(PerfilBuilder builder)
    {
        var seguidor = builder.ComUsuarioId(_userId).Build();
        _uow.ComPerfilDoUsuario(seguidor);
        return seguidor;
    }

    private PerfilEntity ConfigurarSeguido(PerfilBuilder builder)
    {
        var seguido = builder.Build();
        _uow.Perfil.GetByIdAsync(seguido.Id, Arg.Any<CancellationToken>()).Returns(seguido);
        return seguido;
    }

    [Fact]
    public async Task Handle_QuandoAindaNaoSegue_DeveAdicionarRelacaoEntreSeguidorESeguido()
    {
        var seguidor = ConfigurarSeguidor(PerfilBuilder.Investidor());
        var seguido = ConfigurarSeguido(PerfilBuilder.Startup());

        await _handler.Handle(new SeguirPerfilCommand(seguido.Id, _userId), _ct);

        _uow.PerfilSeguidor.Received(1).Add(Arg.Is<PerfilSeguidorEntity>(ps => ps.Seguidor == seguidor && ps.Seguido == seguido));
        await _uow.DeveTerCommitadoUmaVez(_ct);
    }

    [Fact]
    public async Task Handle_QuandoAindaNaoSegue_DeveIncrementarSeguindoESeguidores()
    {
        var seguidor = ConfigurarSeguidor(PerfilBuilder.Investidor().ComTotalSeguindo(2));
        var seguido = ConfigurarSeguido(PerfilBuilder.Startup().ComTotalSeguidores(5));

        await _handler.Handle(new SeguirPerfilCommand(seguido.Id, _userId), _ct);

        seguidor.TotalSeguindo.Total.ShouldBe(3);
        seguido.TotalSeguidores.Total.ShouldBe(6);
    }

    [Fact]
    public async Task Handle_DeveVerificarSeguimentoExistentePelosIdsDosPerfis()
    {
        var seguidor = ConfigurarSeguidor(PerfilBuilder.Investidor());
        var seguido = ConfigurarSeguido(PerfilBuilder.Startup());

        await _handler.Handle(new SeguirPerfilCommand(seguido.Id, _userId), _ct);

        await _uow.Perfil.Received(1).GetByIdAsync(seguido.Id, _ct);
        await _uow.PerfilSeguidor.Received(1).JaSegueAsync(seguidor.Id, seguido.Id, _ct);
    }

    [Fact]
    public async Task Handle_QuandoSeguidorNaoEncontrado_DeveLancarPerfilNaoEncontradoExceptionSemBuscarSeguido()
    {
        _uow.SemPerfilDoUsuario(_userId);

        var acao = () => _handler.Handle(new SeguirPerfilCommand(Guid.NewGuid(), _userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
        await _uow.Perfil.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoSeguidoNaoEncontrado_DeveLancarPerfilNaoEncontradoException()
    {
        ConfigurarSeguidor(PerfilBuilder.Investidor());
        var seguidoId = Guid.NewGuid();
        _uow.Perfil.GetByIdAsync(seguidoId, Arg.Any<CancellationToken>()).Returns((PerfilEntity?)null);

        var acao = () => _handler.Handle(new SeguirPerfilCommand(seguidoId, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
        await _uow.PerfilSeguidor.DidNotReceive().JaSegueAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoJaSegue_DeveLancarPerfilJaSeguidoException()
    {
        var seguidor = ConfigurarSeguidor(PerfilBuilder.Investidor());
        var seguido = ConfigurarSeguido(PerfilBuilder.Startup());
        _uow.PerfilSeguidor.JaSegueAsync(seguidor.Id, seguido.Id, Arg.Any<CancellationToken>()).Returns(true);

        var acao = () => _handler.Handle(new SeguirPerfilCommand(seguido.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilJaSeguidoException>();
        seguido.TotalSeguidores.Total.ShouldBe(0);
        _uow.PerfilSeguidor.DidNotReceive().Add(Arg.Any<PerfilSeguidorEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilTentaSeguirASiMesmo_DeveLancarPerfilNaoPodeSeguirASiMesmoException()
    {
        var perfil = ConfigurarSeguidor(PerfilBuilder.Startup());
        _uow.Perfil.GetByIdAsync(perfil.Id, Arg.Any<CancellationToken>()).Returns(perfil);

        var acao = () => _handler.Handle(new SeguirPerfilCommand(perfil.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoPodeSeguirASiMesmoException>();
        _uow.PerfilSeguidor.DidNotReceive().Add(Arg.Any<PerfilSeguidorEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoSeguidorDesativado_DeveLancarPerfilDesativadoException()
    {
        ConfigurarSeguidor(PerfilBuilder.Investidor().Desativado());
        var seguido = ConfigurarSeguido(PerfilBuilder.Startup());

        var acao = () => _handler.Handle(new SeguirPerfilCommand(seguido.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilDesativadoException>();
        _uow.PerfilSeguidor.DidNotReceive().Add(Arg.Any<PerfilSeguidorEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoSeguidoDesativado_DeveLancarPerfilDesativadoException()
    {
        ConfigurarSeguidor(PerfilBuilder.Investidor());
        var seguido = ConfigurarSeguido(PerfilBuilder.Startup().Desativado());

        var acao = () => _handler.Handle(new SeguirPerfilCommand(seguido.Id, _userId), _ct);

        await acao.ShouldThrowAsync<PerfilDesativadoException>();
        _uow.PerfilSeguidor.DidNotReceive().Add(Arg.Any<PerfilSeguidorEntity>());
        await _uow.NaoDeveTerCommitado();
    }
}
