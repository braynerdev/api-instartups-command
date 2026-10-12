using Instartups.Command.Application.UnitTests.Fixtures;
using Instartups.Command.Application.UseCases.EditarPerfilInvestidor;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.ValueObjects;
using Instartups.Command.Tests.Common.Builders.Commands;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Application.UnitTests.UseCases.EditarPerfilInvestidor;

public class EditarPerfilInvestidorCommandHandlerTests
{
    private readonly UnitOfWorkMock _uow = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly EditarPerfilInvestidorCommandHandler _handler;

    public EditarPerfilInvestidorCommandHandlerTests()
    {
        _handler = new EditarPerfilInvestidorCommandHandler(_uow.UnitOfWork);
    }

    private void ConfigurarPerfil(PerfilEntity? perfil) =>
        _uow.Perfil.ObterPorUsuarioIdComInvestidorAsync(_userId, Arg.Any<CancellationToken>()).Returns(perfil);

    [Fact]
    public async Task Handle_QuandoPerfilInvestidorAtivo_DeveAtualizarPerfilEInvestidorComDadosDoCommand()
    {
        var perfil = PerfilBuilder.Investidor().ComUsuarioId(_userId).Build();
        ConfigurarPerfil(perfil);
        var command = PerfilCommandBuilder.EditarInvestidor(_userId);

        await _handler.Handle(command, _ct);

        perfil.Nome.Nome.ShouldBe(command.Perfil.Nome);
        perfil.Coordenada.ShouldBe(CoordenadaVO.Create(command.Perfil.Coordenada.Latitude, command.Perfil.Coordenada.Longitude));
        perfil.ImagemPerfilUrl.ShouldBe(command.Perfil.ImagemPerfilUrl);
        perfil.ImagemFundoUrl.ShouldBe(command.Perfil.ImagemFundoUrl);
        perfil.Investidor!.TeseInvestimento.ShouldBe(command.TeseInvestimento);
        perfil.Investidor.TicketMinimo.ShouldBe(command.TicketMinimo);
        perfil.Investidor.TicketMaximo.ShouldBe(command.TicketMaximo);
    }

    [Fact]
    public async Task Handle_QuandoPerfilInvestidorAtivo_DeveBuscarPerfilComInvestidorECommitar()
    {
        ConfigurarPerfil(PerfilBuilder.Investidor().ComUsuarioId(_userId).Build());

        await _handler.Handle(PerfilCommandBuilder.EditarInvestidor(_userId), _ct);

        await _uow.Perfil.Received(1).ObterPorUsuarioIdComInvestidorAsync(_userId, _ct);
        await _uow.DeveTerCommitadoUmaVez(_ct);
    }

    [Fact]
    public async Task Handle_QuandoPerfilNaoEncontrado_DeveLancarPerfilNaoEncontradoException()
    {
        ConfigurarPerfil(null);

        var acao = () => _handler.Handle(PerfilCommandBuilder.EditarInvestidor(_userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilEhStartup_DeveLancarPerfilNaoEhInvestidorException()
    {
        ConfigurarPerfil(PerfilBuilder.Startup().ComUsuarioId(_userId).Build());

        var acao = () => _handler.Handle(PerfilCommandBuilder.EditarInvestidor(_userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEhInvestidorException>();
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilDesativado_DeveLancarPerfilDesativadoException()
    {
        ConfigurarPerfil(PerfilBuilder.Investidor().ComUsuarioId(_userId).Desativado().Build());

        var acao = () => _handler.Handle(PerfilCommandBuilder.EditarInvestidor(_userId), _ct);

        await acao.ShouldThrowAsync<PerfilDesativadoException>();
        await _uow.NaoDeveTerCommitado();
    }
}
