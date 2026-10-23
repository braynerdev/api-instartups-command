using Instartups.Command.Application.UnitTests.Fixtures;
using Instartups.Command.Application.UseCases.CadastrarPerfilInvestidor;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.ValueObjects;
using Instartups.Command.Tests.Common.Builders.Commands;

namespace Instartups.Command.Application.UnitTests.UseCases.CadastrarPerfilInvestidor;

public class CadastrarPerfilInvestidorCommandHandlerTests
{
    private readonly UnitOfWorkMock _uow = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly CadastrarPerfilInvestidorCommandHandler _handler;

    public CadastrarPerfilInvestidorCommandHandlerTests()
    {
        _handler = new CadastrarPerfilInvestidorCommandHandler(_uow.UnitOfWork);
    }

    [Fact]
    public async Task Handle_QuandoUsuarioNaoPossuiPerfil_DeveAdicionarPerfilInvestidorComDadosDoCommand()
    {
        var command = PerfilCommandBuilder.CadastrarInvestidor();
        PerfilEntity? perfilAdicionado = null;
        _uow.Perfil.Add(Arg.Do<PerfilEntity>(p => perfilAdicionado = p));

        await _handler.Handle(command, _ct);

        perfilAdicionado.ShouldNotBeNull();
        perfilAdicionado.TipoPerfil.ShouldBe(TiposPerfisEnum.Investidor);
        perfilAdicionado.UsuarioId.ShouldBe(command.Perfil.UserId);
        perfilAdicionado.Nome.Nome.ShouldBe(command.Perfil.Nome);
        perfilAdicionado.Coordenada.ShouldBe(CoordenadaVO.Create(command.Perfil.Coordenada.Latitude, command.Perfil.Coordenada.Longitude));
        perfilAdicionado.ImagemPerfilUrl.ShouldBe(command.Perfil.ImagemPerfilUrl);
        perfilAdicionado.ImagemFundoUrl.ShouldBe(command.Perfil.ImagemFundoUrl);
        perfilAdicionado.Investidor.ShouldNotBeNull();
        perfilAdicionado.Investidor.TeseInvestimento.ShouldBe(command.TeseInvestimento);
        perfilAdicionado.Investidor.TicketMinimo.ShouldBe(command.TicketMinimo);
        perfilAdicionado.Investidor.TicketMaximo.ShouldBe(command.TicketMaximo);
    }

    [Fact]
    public async Task Handle_QuandoUsuarioNaoPossuiPerfil_DeveRetornarIdDoPerfilCriado()
    {
        PerfilEntity? perfilAdicionado = null;
        _uow.Perfil.Add(Arg.Do<PerfilEntity>(p => perfilAdicionado = p));

        var resposta = await _handler.Handle(PerfilCommandBuilder.CadastrarInvestidor(), _ct);

        resposta.Id.ShouldBe(perfilAdicionado!.Id);
    }

    [Fact]
    public async Task Handle_QuandoUsuarioNaoPossuiPerfil_DeveAdicionarECommitarNessaOrdem()
    {
        var command = PerfilCommandBuilder.CadastrarInvestidor();

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
        var command = PerfilCommandBuilder.CadastrarInvestidor();
        _uow.Perfil.UsuarioJaPossuiPerfilAsync(command.Perfil.UserId, Arg.Any<CancellationToken>()).Returns(true);

        var acao = () => _handler.Handle(command, _ct);

        await acao.ShouldThrowAsync<UsuarioJaPossuiPerfilException>();
        _uow.Perfil.DidNotReceive().Add(Arg.Any<PerfilEntity>());
        await _uow.NaoDeveTerCommitado();
    }
}
