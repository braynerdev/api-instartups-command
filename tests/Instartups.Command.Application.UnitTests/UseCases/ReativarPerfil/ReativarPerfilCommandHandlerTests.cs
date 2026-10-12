using Instartups.Command.Application.UnitTests.Fixtures;
using Instartups.Command.Application.UseCases.ReativarPerfil;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Application.UnitTests.UseCases.ReativarPerfil;

public class ReativarPerfilCommandHandlerTests
{
    private readonly UnitOfWorkMock _uow = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly ReativarPerfilCommandHandler _handler;

    public ReativarPerfilCommandHandlerTests()
    {
        _handler = new ReativarPerfilCommandHandler(_uow.UnitOfWork);
    }

    [Fact]
    public async Task Handle_QuandoPerfilDesativado_DeveReativarPerfilECommitar()
    {
        var perfil = PerfilBuilder.Startup().ComUsuarioId(_userId).Desativado().Build();
        _uow.ComPerfilDoUsuario(perfil);

        await _handler.Handle(new ReativarPerfilCommand(_userId), _ct);

        perfil.Ativo.ShouldBeTrue();
        await _uow.DeveTerCommitadoUmaVez(_ct);
    }

    [Fact]
    public async Task Handle_QuandoPerfilNaoEncontrado_DeveLancarPerfilNaoEncontradoException()
    {
        _uow.SemPerfilDoUsuario(_userId);

        var acao = () => _handler.Handle(new ReativarPerfilCommand(_userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilJaAtivo_DeveLancarPerfilJaAtivoException()
    {
        _uow.ComPerfilDoUsuario(PerfilBuilder.Startup().ComUsuarioId(_userId).Build());

        var acao = () => _handler.Handle(new ReativarPerfilCommand(_userId), _ct);

        await acao.ShouldThrowAsync<PerfilJaAtivoException>();
        await _uow.NaoDeveTerCommitado();
    }
}
