using Instartups.Command.Application.UnitTests.Fixtures;
using Instartups.Command.Application.UseCases.DesativarPerfil;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Application.UnitTests.UseCases.DesativarPerfil;

public class DesativarPerfilCommandHandlerTests
{
    private readonly UnitOfWorkMock _uow = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly DesativarPerfilCommandHandler _handler;

    public DesativarPerfilCommandHandlerTests()
    {
        _handler = new DesativarPerfilCommandHandler(_uow.UnitOfWork);
    }

    [Fact]
    public async Task Handle_QuandoPerfilAtivo_DeveDesativarPerfilECommitar()
    {
        var perfil = PerfilBuilder.Startup().ComUsuarioId(_userId).Build();
        _uow.ComPerfilDoUsuario(perfil);

        await _handler.Handle(new DesativarPerfilCommand(_userId), _ct);

        perfil.Ativo.ShouldBeFalse();
        await _uow.DeveTerCommitadoUmaVez(_ct);
    }

    [Fact]
    public async Task Handle_QuandoPerfilNaoEncontrado_DeveLancarPerfilNaoEncontradoException()
    {
        _uow.SemPerfilDoUsuario(_userId);

        var acao = () => _handler.Handle(new DesativarPerfilCommand(_userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilJaDesativado_DeveLancarPerfilJaDesativadoException()
    {
        _uow.ComPerfilDoUsuario(PerfilBuilder.Startup().ComUsuarioId(_userId).Desativado().Build());

        var acao = () => _handler.Handle(new DesativarPerfilCommand(_userId), _ct);

        await acao.ShouldThrowAsync<PerfilJaDesativadoException>();
        await _uow.NaoDeveTerCommitado();
    }
}
