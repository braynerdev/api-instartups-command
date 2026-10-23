using Instartups.Command.Application.UnitTests.Fixtures;
using Instartups.Command.Application.UseCases.CadastrarPerfilStartup;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.ValueObjects;
using Instartups.Command.Tests.Common.Builders.Commands;

namespace Instartups.Command.Application.UnitTests.UseCases.CadastrarPerfilStartup;

public class CadastrarPerfilStartupCommandHandlerTests
{
    private readonly UnitOfWorkMock _uow = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly CadastrarPerfilStartupCommandHandler _handler;

    public CadastrarPerfilStartupCommandHandlerTests()
    {
        _handler = new CadastrarPerfilStartupCommandHandler(_uow.UnitOfWork);
    }

    [Fact]
    public async Task Handle_QuandoUsuarioNaoPossuiPerfil_DeveAdicionarPerfilStartupComDadosDoCommand()
    {
        var command = PerfilCommandBuilder.CadastrarStartup();
        PerfilEntity? perfilAdicionado = null;
        _uow.Perfil.Add(Arg.Do<PerfilEntity>(p => perfilAdicionado = p));

        await _handler.Handle(command, _ct);

        perfilAdicionado.ShouldNotBeNull();
        perfilAdicionado.TipoPerfil.ShouldBe(TiposPerfisEnum.Startup);
        perfilAdicionado.UsuarioId.ShouldBe(command.Perfil.UserId);
        perfilAdicionado.Nome.Nome.ShouldBe(command.Perfil.Nome);
        perfilAdicionado.Coordenada.ShouldBe(CoordenadaVO.Create(command.Perfil.Coordenada.Latitude, command.Perfil.Coordenada.Longitude));
        perfilAdicionado.ImagemPerfilUrl.ShouldBe(command.Perfil.ImagemPerfilUrl);
        perfilAdicionado.ImagemFundoUrl.ShouldBe(command.Perfil.ImagemFundoUrl);
        perfilAdicionado.Startup.ShouldNotBeNull();
        perfilAdicionado.Startup.Pitch.ShouldBe(command.Pitch);
        perfilAdicionado.Startup.DataFundacao.ShouldBe(command.DataFundacao);
        perfilAdicionado.Startup.TamanhoEquipe.ShouldBe(command.TamanhoEquipe);
        perfilAdicionado.Startup.ValorBuscado.ShouldBe(command.ValorBuscado);
        perfilAdicionado.Startup.Setor.ShouldBe(command.Setor);
    }

    [Fact]
    public async Task Handle_QuandoUsuarioNaoPossuiPerfil_DeveRetornarIdDoPerfilCriado()
    {
        PerfilEntity? perfilAdicionado = null;
        _uow.Perfil.Add(Arg.Do<PerfilEntity>(p => perfilAdicionado = p));

        var resposta = await _handler.Handle(PerfilCommandBuilder.CadastrarStartup(), _ct);

        resposta.Id.ShouldBe(perfilAdicionado!.Id);
    }

    [Fact]
    public async Task Handle_QuandoUsuarioNaoPossuiPerfil_DeveAdicionarECommitarNessaOrdem()
    {
        var command = PerfilCommandBuilder.CadastrarStartup();

        await _handler.Handle(command, _ct);

        Received.InOrder(() =>
        {
            _uow.Perfil.UsuarioJaPossuiPerfilAsync(command.Perfil.UserId, _ct);
            _uow.Perfil.Add(Arg.Any<PerfilEntity>());
            _uow.UnitOfWork.CommitAsync(_ct);
        });
        await _uow.DeveTerCommitadoUmaVez(_ct);
    }

    [Fact]
    public async Task Handle_QuandoUsuarioJaPossuiPerfil_DeveLancarUsuarioJaPossuiPerfilException()
    {
        var command = PerfilCommandBuilder.CadastrarStartup();
        _uow.Perfil.UsuarioJaPossuiPerfilAsync(command.Perfil.UserId, Arg.Any<CancellationToken>()).Returns(true);

        var acao = () => _handler.Handle(command, _ct);

        await acao.ShouldThrowAsync<UsuarioJaPossuiPerfilException>();
        _uow.Perfil.DidNotReceive().Add(Arg.Any<PerfilEntity>());
        await _uow.NaoDeveTerCommitado();
    }
}
