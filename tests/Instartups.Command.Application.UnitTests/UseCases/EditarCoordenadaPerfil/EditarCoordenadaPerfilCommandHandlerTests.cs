using Instartups.Command.Application.UnitTests.Fixtures;
using Instartups.Command.Application.UseCases.EditarCoordenadaPerfil;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.ValueObjects;
using Instartups.Command.Tests.Common.Builders.Commands;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Application.UnitTests.UseCases.EditarCoordenadaPerfil;

public class EditarCoordenadaPerfilCommandHandlerTests
{
    private readonly UnitOfWorkMock _uow = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly EditarCoordenadaPerfilCommandHandler _handler;

    public EditarCoordenadaPerfilCommandHandlerTests()
    {
        _handler = new EditarCoordenadaPerfilCommandHandler(_uow.UnitOfWork);
    }

    [Fact]
    public async Task Handle_QuandoPerfilAtivo_DeveAtualizarCoordenadaECommitar()
    {
        var perfil = PerfilBuilder.Startup().ComUsuarioId(_userId).Build();
        _uow.ComPerfilDoUsuario(perfil);
        var command = PerfilCommandBuilder.EditarCoordenada(_userId);

        await _handler.Handle(command, _ct);

        perfil.Coordenada.ShouldBe(CoordenadaVO.Create(command.Coordenada.Latitude, command.Coordenada.Longitude));
        await _uow.DeveTerCommitadoUmaVez(_ct);
    }

    [Fact]
    public async Task Handle_QuandoPerfilNaoEncontrado_DeveLancarPerfilNaoEncontradoException()
    {
        _uow.SemPerfilDoUsuario(_userId);

        var acao = () => _handler.Handle(PerfilCommandBuilder.EditarCoordenada(_userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilDesativado_DeveLancarPerfilDesativadoException()
    {
        _uow.ComPerfilDoUsuario(PerfilBuilder.Startup().ComUsuarioId(_userId).Desativado().Build());

        var acao = () => _handler.Handle(PerfilCommandBuilder.EditarCoordenada(_userId), _ct);

        await acao.ShouldThrowAsync<PerfilDesativadoException>();
        await _uow.NaoDeveTerCommitado();
    }
}
