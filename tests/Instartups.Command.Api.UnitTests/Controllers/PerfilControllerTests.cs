using Instartups.Command.Api.Controllers;
using Instartups.Command.Api.DTOs;
using Instartups.Command.Api.UnitTests.Builders;
using Instartups.Command.Api.UnitTests.Fixtures;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Common.Response;
using Instartups.Command.Application.Interfaces;
using Instartups.Command.Application.UseCases.CadastrarPerfilInvestidor;
using Instartups.Command.Application.UseCases.CadastrarPerfilStartup;
using Instartups.Command.Application.UseCases.DeixarDeSeguirPerfil;
using Instartups.Command.Application.UseCases.DesativarPerfil;
using Instartups.Command.Application.UseCases.EditarCoordenadaPerfil;
using Instartups.Command.Application.UseCases.EditarPerfilInvestidor;
using Instartups.Command.Application.UseCases.EditarPerfilStartup;
using Instartups.Command.Application.UseCases.ReativarPerfil;
using Instartups.Command.Application.UseCases.SeguirPerfil;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Instartups.Command.Api.UnitTests.Controllers;

public class PerfilControllerTests
{
    private readonly IPegarContextoRequisicao _contexto = Substitute.For<IPegarContextoRequisicao>();
    private readonly IMessageBus _messageBus = Substitute.For<IMessageBus>();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly PerfilController _controller;

    public PerfilControllerTests()
    {
        _contexto.UserId.Returns(_userId);
        _controller = new PerfilController(_contexto, _messageBus);
    }

    private IdResponse ConfigurarRespostaDeCadastro()
    {
        var resposta = new IdResponse(Guid.NewGuid());
        _messageBus.InvokeAsync<IdResponse>(Arg.Any<object>(), Arg.Any<CancellationToken>(), Arg.Any<TimeSpan?>()).Returns(resposta);
        return resposta;
    }

    private static void DeveSerCreatedComId(ActionResult<ResponseDTO<IdResponse>> resultado, IdResponse idEsperado)
    {
        var objectResult = resultado.Result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status201Created);
        var corpo = objectResult.Value.ShouldBeOfType<ResponseDTO<IdResponse>>();
        corpo.IsSuccess.ShouldBeTrue();
        corpo.Message.ShouldBe(MensagensSucessoConst.PerfilCadastrado);
        corpo.Data.ShouldBe(idEsperado);
    }

    #region Cadastrar

    [Fact]
    public async Task CadastrarPerfilInvestidor_DeveEnviarCommandComUserIdDoContextoERetornar201()
    {
        var dto = DtoBuilder.CadastrarInvestidor();
        var idCriado = ConfigurarRespostaDeCadastro();

        var resultado = await _controller.CadastrarPerfilInvestidor(dto, _ct);

        DeveSerCreatedComId(resultado, idCriado);
        _messageBus.UnicoComandoEnviado<CadastrarPerfilInvestidorCommand>(_ct).ShouldBe(dto.ToCommand(_userId));
    }

    [Fact]
    public async Task CadastrarPerfilStartup_DeveEnviarCommandComUserIdDoContextoERetornar201()
    {
        var dto = DtoBuilder.CadastrarStartup();
        var idCriado = ConfigurarRespostaDeCadastro();

        var resultado = await _controller.CadastrarPerfilStartup(dto, _ct);

        DeveSerCreatedComId(resultado, idCriado);
        _messageBus.UnicoComandoEnviado<CadastrarPerfilStartupCommand>(_ct).ShouldBe(dto.ToCommand(_userId));
    }

    [Fact]
    public async Task CadastrarPerfilStartup_QuandoMessageBusLancaExcecao_DevePropagarExcecao()
    {
        _messageBus.InvokeAsync<IdResponse>(Arg.Any<object>(), Arg.Any<CancellationToken>(), Arg.Any<TimeSpan?>())
            .Returns(Task.FromException<IdResponse>(new UsuarioJaPossuiPerfilException()));

        var acao = () => _controller.CadastrarPerfilStartup(DtoBuilder.CadastrarStartup(), _ct);

        await acao.ShouldThrowAsync<UsuarioJaPossuiPerfilException>();
    }

    #endregion

    #region Editar

    [Fact]
    public async Task EditarPerfilInvestidor_DeveEnviarCommandComUserIdDoContextoERetornar204()
    {
        var dto = DtoBuilder.EditarInvestidor();

        var resultado = await _controller.EditarPerfilInvestidor(dto, _ct);

        resultado.ShouldBeOfType<NoContentResult>();
        _messageBus.UnicoComandoEnviado<EditarPerfilInvestidorCommand>(_ct).ShouldBe(dto.ToCommand(_userId));
    }

    [Fact]
    public async Task EditarPerfilStartup_DeveEnviarCommandComUserIdDoContextoERetornar204()
    {
        var dto = DtoBuilder.EditarStartup();

        var resultado = await _controller.EditarPerfilStartup(dto, _ct);

        resultado.ShouldBeOfType<NoContentResult>();
        _messageBus.UnicoComandoEnviado<EditarPerfilStartupCommand>(_ct).ShouldBe(dto.ToCommand(_userId));
    }

    [Fact]
    public async Task EditarCoordenadaPerfil_DeveEnviarCommandComCoordenadaEUserIdERetornar204()
    {
        var dto = new CoordenadaDTO(-15.79, -47.88);

        var resultado = await _controller.EditarCoordenadaPerfil(dto, _ct);

        resultado.ShouldBeOfType<NoContentResult>();
        _messageBus.UnicoComandoEnviado<EditarCoordenadaPerfilCommand>(_ct)
            .ShouldBe(new EditarCoordenadaPerfilCommand(new CoordenadaCommand(-15.79, -47.88), _userId));
    }

    [Fact]
    public async Task EditarPerfilStartup_QuandoMessageBusLancaExcecao_DevePropagarExcecao()
    {
        _messageBus.InvokeAsync(Arg.Any<object>(), Arg.Any<CancellationToken>(), Arg.Any<TimeSpan?>())
            .Returns(Task.FromException(new PerfilNaoEncontradoException()));

        var acao = () => _controller.EditarPerfilStartup(DtoBuilder.EditarStartup(), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
    }

    #endregion

    #region Status

    [Fact]
    public async Task DesativarPerfil_DeveEnviarCommandComUserIdDoContextoERetornar204()
    {
        var resultado = await _controller.DesativarPerfil(_ct);

        resultado.ShouldBeOfType<NoContentResult>();
        _messageBus.UnicoComandoEnviado<DesativarPerfilCommand>(_ct).ShouldBe(new DesativarPerfilCommand(_userId));
    }

    [Fact]
    public async Task ReativarPerfil_DeveEnviarCommandComUserIdDoContextoERetornar204()
    {
        var resultado = await _controller.ReativarPerfil(_ct);

        resultado.ShouldBeOfType<NoContentResult>();
        _messageBus.UnicoComandoEnviado<ReativarPerfilCommand>(_ct).ShouldBe(new ReativarPerfilCommand(_userId));
    }

    #endregion

    #region Seguir

    [Fact]
    public async Task SeguirPerfil_DeveEnviarCommandComSeguidoDaRotaEUserIdERetornar204()
    {
        var seguidoId = Guid.NewGuid();

        var resultado = await _controller.SeguirPerfil(seguidoId, _ct);

        resultado.ShouldBeOfType<NoContentResult>();
        _messageBus.UnicoComandoEnviado<SeguirPerfilCommand>(_ct).ShouldBe(new SeguirPerfilCommand(seguidoId, _userId));
    }

    [Fact]
    public async Task DeixarDeSeguirPerfil_DeveEnviarCommandComSeguidoDaRotaEUserIdERetornar204()
    {
        var seguidoId = Guid.NewGuid();

        var resultado = await _controller.DeixarDeSeguirPerfil(seguidoId, _ct);

        resultado.ShouldBeOfType<NoContentResult>();
        _messageBus.UnicoComandoEnviado<DeixarDeSeguirPerfilCommand>(_ct).ShouldBe(new DeixarDeSeguirPerfilCommand(seguidoId, _userId));
    }

    #endregion
}
