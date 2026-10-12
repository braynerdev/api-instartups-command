using Instartups.Command.Api.Controllers;
using Instartups.Command.Api.DTOs;
using Instartups.Command.Api.UnitTests.Builders;
using Instartups.Command.Api.UnitTests.Fixtures;
using Instartups.Command.Application.Common.Response;
using Instartups.Command.Application.Interfaces;
using Instartups.Command.Application.UseCases.CadastrarPostagem;
using Instartups.Command.Application.UseCases.CurtirPostagem;
using Instartups.Command.Application.UseCases.DescurtirPostagem;
using Instartups.Command.Application.UseCases.DesativarPostagem;
using Instartups.Command.Application.UseCases.EditarPostagem;
using Instartups.Command.Application.UseCases.ReativarPostagem;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Instartups.Command.Api.UnitTests.Controllers;

public class PostagemControllerTests
{
    private readonly IPegarContextoRequisicao _contexto = Substitute.For<IPegarContextoRequisicao>();
    private readonly IMessageBus _messageBus = Substitute.For<IMessageBus>();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _postagemId = Guid.NewGuid();
    private readonly PostagemController _controller;

    public PostagemControllerTests()
    {
        _contexto.UserId.Returns(_userId);
        _controller = new PostagemController(_contexto, _messageBus);
    }

    #region Cadastrar

    [Fact]
    public async Task Post_DeveEnviarCommandComUserIdComoAutorERetornar201()
    {
        var dto = DtoBuilder.CadastrarPostagem();
        var idCriado = new IdResponse(Guid.NewGuid());
        _messageBus.InvokeAsync<IdResponse>(Arg.Any<object>(), Arg.Any<CancellationToken>(), Arg.Any<TimeSpan?>()).Returns(idCriado);

        var resultado = await _controller.Post(dto, _ct);

        var objectResult = resultado.Result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status201Created);
        var corpo = objectResult.Value.ShouldBeOfType<ResponseDTO<IdResponse>>();
        corpo.IsSuccess.ShouldBeTrue();
        corpo.Message.ShouldBe(MensagensSucessoConst.PostagemCadastrada);
        corpo.Data.ShouldBe(idCriado);

        var command = _messageBus.UnicoComandoEnviado<CadastrarPostagemCommand>(_ct);
        command.AutorId.ShouldBe(_userId);
        command.Descricao.ShouldBe(dto.Descricao);
        command.Midias.ShouldBe(dto.ToCommand(_userId).Midias);
    }

    [Fact]
    public async Task Post_QuandoMessageBusLancaExcecao_DevePropagarExcecao()
    {
        _messageBus.InvokeAsync<IdResponse>(Arg.Any<object>(), Arg.Any<CancellationToken>(), Arg.Any<TimeSpan?>())
            .Returns(Task.FromException<IdResponse>(new PerfilNaoEncontradoException()));

        var acao = () => _controller.Post(DtoBuilder.CadastrarPostagem(), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
    }

    #endregion

    #region Editar

    [Fact]
    public async Task Editar_DeveEnviarCommandComPostagemDaRotaEUserIdERetornar204()
    {
        var dto = DtoBuilder.EditarPostagem();

        var resultado = await _controller.Editar(_postagemId, dto, _ct);

        resultado.ShouldBeOfType<NoContentResult>();
        var command = _messageBus.UnicoComandoEnviado<EditarPostagemCommand>(_ct);
        command.PostagemId.ShouldBe(_postagemId);
        command.UserId.ShouldBe(_userId);
        command.Descricao.ShouldBe(dto.Descricao);
        command.MidiasRemovidas.ShouldBe(dto.MidiasRemovidas!);
        command.MidiasAdicionadas.ShouldBe(dto.ToCommand(_postagemId, _userId).MidiasAdicionadas);
    }

    [Fact]
    public async Task Editar_QuandoMessageBusLancaExcecao_DevePropagarExcecao()
    {
        _messageBus.InvokeAsync(Arg.Any<object>(), Arg.Any<CancellationToken>(), Arg.Any<TimeSpan?>())
            .Returns(Task.FromException(new PerfilNaoEhAutorPostagemException()));

        var acao = () => _controller.Editar(_postagemId, DtoBuilder.EditarPostagem(), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEhAutorPostagemException>();
    }

    #endregion

    #region Status e curtidas

    [Fact]
    public async Task Desativar_DeveEnviarCommandComPostagemDaRotaEUserIdERetornar204()
    {
        var resultado = await _controller.Desativar(_postagemId, _ct);

        resultado.ShouldBeOfType<NoContentResult>();
        _messageBus.UnicoComandoEnviado<DesativarPostagemCommand>(_ct).ShouldBe(new DesativarPostagemCommand(_postagemId, _userId));
    }

    [Fact]
    public async Task Reativar_DeveEnviarCommandComPostagemDaRotaEUserIdERetornar204()
    {
        var resultado = await _controller.Reativar(_postagemId, _ct);

        resultado.ShouldBeOfType<NoContentResult>();
        _messageBus.UnicoComandoEnviado<ReativarPostagemCommand>(_ct).ShouldBe(new ReativarPostagemCommand(_postagemId, _userId));
    }

    [Fact]
    public async Task Curtir_DeveEnviarCommandComPostagemDaRotaEUserIdERetornar204()
    {
        var resultado = await _controller.Curtir(_postagemId, _ct);

        resultado.ShouldBeOfType<NoContentResult>();
        _messageBus.UnicoComandoEnviado<CurtirPostagemCommand>(_ct).ShouldBe(new CurtirPostagemCommand(_postagemId, _userId));
    }

    [Fact]
    public async Task Descurtir_DeveEnviarCommandComPostagemDaRotaEUserIdERetornar204()
    {
        var resultado = await _controller.Descurtir(_postagemId, _ct);

        resultado.ShouldBeOfType<NoContentResult>();
        _messageBus.UnicoComandoEnviado<DescurtirPostagemCommand>(_ct).ShouldBe(new DescurtirPostagemCommand(_postagemId, _userId));
    }

    #endregion
}
