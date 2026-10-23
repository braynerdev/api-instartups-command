using System.Text.Json;
using FluentValidation.Results;
using Instartups.Command.Api.Extensions;
using Instartups.Command.Api.Middleware;
using Instartups.Command.Api.UnitTests.Fixtures;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.Exceptions.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Instartups.Command.Api.UnitTests.Middleware;

public class ExceptionsMiddlewareTests
{
    private readonly LoggerSpy<ExceptionsMiddleware> _logger = new();

    private sealed class ValidacaoDominioException() : ValidationException("Valor inválido no domínio.");

    private static DefaultHttpContext CriarContexto() => new() { Response = { Body = new MemoryStream() } };

    private async Task<DefaultHttpContext> ExecutarLancando(Exception excecao)
    {
        var contexto = CriarContexto();
        var middleware = new ExceptionsMiddleware(_ => throw excecao, _logger);

        await middleware.InvokeAsync(contexto);

        return contexto;
    }

    private static JsonElement LerCorpo(HttpContext contexto)
    {
        contexto.Response.Body.Position = 0;
        return JsonDocument.Parse(contexto.Response.Body).RootElement;
    }

    #region Sem exceção

    [Fact]
    public async Task InvokeAsync_QuandoProximoMiddlewareNaoLancaExcecao_DeveExecutarProximoSemAlterarResposta()
    {
        var contexto = CriarContexto();
        var proximoExecutado = false;
        var middleware = new ExceptionsMiddleware(_ => { proximoExecutado = true; return Task.CompletedTask; }, _logger);

        await middleware.InvokeAsync(contexto);

        proximoExecutado.ShouldBeTrue();
        contexto.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
        contexto.Response.Body.Length.ShouldBe(0);
        _logger.Registros.ShouldBeEmpty();
    }

    #endregion

    #region Status code

    [Theory]
    [InlineData(typeof(ValidacaoDominioException), StatusCodes.Status400BadRequest)]
    [InlineData(typeof(PerfilNaoEhAutorPostagemException), StatusCodes.Status403Forbidden)]
    [InlineData(typeof(PerfilNaoEncontradoException), StatusCodes.Status404NotFound)]
    [InlineData(typeof(PostagemNaoEncontradaException), StatusCodes.Status404NotFound)]
    [InlineData(typeof(PostagemNaoCurtidaException), StatusCodes.Status404NotFound)]
    [InlineData(typeof(PerfilNaoSeguidoException), StatusCodes.Status404NotFound)]
    [InlineData(typeof(MidiaPostagemNaoEncontradaException), StatusCodes.Status404NotFound)]
    [InlineData(typeof(UsuarioJaPossuiPerfilException), StatusCodes.Status409Conflict)]
    [InlineData(typeof(PerfilJaAtivoException), StatusCodes.Status409Conflict)]
    [InlineData(typeof(PerfilJaDesativadoException), StatusCodes.Status409Conflict)]
    [InlineData(typeof(PerfilJaSeguidoException), StatusCodes.Status409Conflict)]
    [InlineData(typeof(PostagemJaAtivaException), StatusCodes.Status409Conflict)]
    [InlineData(typeof(PostagemJaDesativadaException), StatusCodes.Status409Conflict)]
    [InlineData(typeof(PostagemJaCurtidaException), StatusCodes.Status409Conflict)]
    [InlineData(typeof(PerfilDesativadoException), StatusCodes.Status422UnprocessableEntity)]
    [InlineData(typeof(PerfilNaoEhStartupException), StatusCodes.Status422UnprocessableEntity)]
    [InlineData(typeof(PerfilNaoEhInvestidorException), StatusCodes.Status422UnprocessableEntity)]
    [InlineData(typeof(PerfilNaoPodeCurtirPropriaPostagemException), StatusCodes.Status422UnprocessableEntity)]
    [InlineData(typeof(PerfilNaoPodeSeguirASiMesmoException), StatusCodes.Status422UnprocessableEntity)]
    [InlineData(typeof(PostagemDesativadaException), StatusCodes.Status422UnprocessableEntity)]
    [InlineData(typeof(TotalNegativoException), StatusCodes.Status422UnprocessableEntity)]
    [InlineData(typeof(OperationCanceledException), StatusCodes.Status499ClientClosedRequest)]
    [InlineData(typeof(TaskCanceledException), StatusCodes.Status499ClientClosedRequest)]
    [InlineData(typeof(Exception), StatusCodes.Status500InternalServerError)]
    [InlineData(typeof(ArgumentException), StatusCodes.Status500InternalServerError)]
    [InlineData(typeof(NullReferenceException), StatusCodes.Status500InternalServerError)]
    [InlineData(typeof(InvalidOperationException), StatusCodes.Status500InternalServerError)]
    public async Task InvokeAsync_QuandoExcecaoLancada_DeveRetornarStatusCodeCorrespondente(Type tipoExcecao, int statusEsperado)
    {
        var excecao = (Exception)Activator.CreateInstance(tipoExcecao, nonPublic: true)!;

        var contexto = await ExecutarLancando(excecao);

        contexto.Response.StatusCode.ShouldBe(statusEsperado);
    }

    [Fact]
    public async Task InvokeAsync_QuandoLimiteMidiasExcedido_DeveRetornar422()
    {
        var contexto = await ExecutarLancando(new LimiteMidiasPostagemExcedidoException(10));

        contexto.Response.StatusCode.ShouldBe(StatusCodes.Status422UnprocessableEntity);
    }

    [Fact]
    public async Task InvokeAsync_QuandoUnauthorizedException_DeveRetornar401()
    {
        var contexto = await ExecutarLancando(new UnauthorizedException("Token inválido."));

        contexto.Response.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_QuandoFluentValidationException_DeveRetornar400()
    {
        var contexto = await ExecutarLancando(new FluentValidation.ValidationException([new ValidationFailure("Nome", "erro")]));

        contexto.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
    }

    #endregion

    #region Corpo da resposta

    [Fact]
    public async Task InvokeAsync_QuandoExcecaoLancada_DeveResponderJson()
    {
        var contexto = await ExecutarLancando(new PostagemNaoEncontradaException());

        contexto.Response.ContentType.ShouldNotBeNull();
        contexto.Response.ContentType.ShouldStartWith("application/json");
    }

    [Fact]
    public async Task InvokeAsync_QuandoExcecaoDeNegocio_DeveRetornarCorpoDeErroComMensagemDaExcecao()
    {
        var contexto = await ExecutarLancando(new PostagemNaoEncontradaException());

        var corpo = LerCorpo(contexto);
        corpo.GetProperty("isSuccess").GetBoolean().ShouldBeFalse();
        corpo.GetProperty("message").GetString().ShouldBe("Postagem não encontrada.");
        corpo.GetProperty("data").ValueKind.ShouldBe(JsonValueKind.Null);
        corpo.GetProperty("dateTimeResponse").GetDateTimeOffset().ShouldBeInRange(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task InvokeAsync_QuandoUnauthorizedException_DeveRetornarMensagemDaExcecao()
    {
        var contexto = await ExecutarLancando(new UnauthorizedException("Token inválido."));

        LerCorpo(contexto).GetProperty("message").GetString().ShouldBe("Token inválido.");
    }

    [Fact]
    public async Task InvokeAsync_QuandoFluentValidationException_DeveRetornarListaDeErrosDeValidacao()
    {
        var falhas = new[]
        {
            new ValidationFailure("Perfil.Nome", "O campo é obrigatório.") { ErrorCode = "CAMPO_OBRIGATORIO" },
            new ValidationFailure("Pitch", "O tamanho do campo deve ser no máximo 1000.") { ErrorCode = "TAMANHO_MAXIMO" }
        };

        var contexto = await ExecutarLancando(new FluentValidation.ValidationException(falhas));

        var corpo = LerCorpo(contexto);
        corpo.GetProperty("isSuccess").GetBoolean().ShouldBeFalse();
        corpo.GetProperty("message").GetString().ShouldBe("Erro de validação.");
        var erros = corpo.GetProperty("data").EnumerateArray().ToList();
        erros.Count.ShouldBe(2);
        erros[0].GetProperty("campo").GetString().ShouldBe("Perfil.Nome");
        erros[0].GetProperty("codigo").GetString().ShouldBe("CAMPO_OBRIGATORIO");
        erros[0].GetProperty("mensagem").GetString().ShouldBe("O campo é obrigatório.");
        erros[1].GetProperty("campo").GetString().ShouldBe("Pitch");
        erros[1].GetProperty("codigo").GetString().ShouldBe("TAMANHO_MAXIMO");
    }

    [Fact]
    public async Task InvokeAsync_QuandoOperacaoCancelada_DeveRetornarMensagemDeCancelamento()
    {
        var contexto = await ExecutarLancando(new OperationCanceledException());

        var corpo = LerCorpo(contexto);
        corpo.GetProperty("isSuccess").GetBoolean().ShouldBeFalse();
        corpo.GetProperty("message").GetString().ShouldBe("Requisição cancelada pelo cliente");
    }

    [Fact]
    public async Task InvokeAsync_QuandoExcecaoInesperada_DeveRetornarMensagemGenericaSemExporDetalhes()
    {
        var contexto = await ExecutarLancando(new InvalidOperationException("Connection string secreta"));

        var corpo = LerCorpo(contexto);
        corpo.GetProperty("isSuccess").GetBoolean().ShouldBeFalse();
        corpo.GetProperty("message").GetString().ShouldBe("Erro inesperado");
        corpo.GetRawText().ShouldNotContain("secreta");
    }

    #endregion

    #region Log

    [Fact]
    public async Task InvokeAsync_QuandoExcecaoInesperada_DeveLogarErroComAExcecao()
    {
        var excecao = new InvalidOperationException("falha");

        await ExecutarLancando(excecao);

        var registro = _logger.Registros.ShouldHaveSingleItem();
        registro.Nivel.ShouldBe(LogLevel.Error);
        registro.Mensagem.ShouldBe("Erro interno da aplicação");
        registro.Excecao.ShouldBeSameAs(excecao);
    }

    [Fact]
    public async Task InvokeAsync_QuandoExcecaoDeNegocio_DeveLogarWarningComAMensagem()
    {
        await ExecutarLancando(new PostagemJaCurtidaException());

        var registro = _logger.Registros.ShouldHaveSingleItem();
        registro.Nivel.ShouldBe(LogLevel.Warning);
        registro.Mensagem.ShouldBe("Erro de negócio: Você já curtiu esta postagem.");
        registro.Excecao.ShouldBeNull();
    }

    [Fact]
    public async Task InvokeAsync_QuandoOperacaoCancelada_DeveLogarWarning()
    {
        await ExecutarLancando(new OperationCanceledException());

        _logger.Registros.ShouldHaveSingleItem().Nivel.ShouldBe(LogLevel.Warning);
    }

    #endregion

    #region Extensão

    [Fact]
    public async Task UseExceptionsMiddleware_DeveRegistrarMiddlewareNoPipeline()
    {
        var servicos = new ServiceCollection().AddLogging().BuildServiceProvider();
        var app = new ApplicationBuilder(servicos);

        var retorno = app.UseExceptionsMiddleware();
        app.Run(_ => throw new PerfilNaoEncontradoException());
        var contexto = CriarContexto();
        contexto.RequestServices = servicos;
        await app.Build()(contexto);

        retorno.ShouldBeSameAs(app);
        contexto.Response.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
    }

    #endregion
}
