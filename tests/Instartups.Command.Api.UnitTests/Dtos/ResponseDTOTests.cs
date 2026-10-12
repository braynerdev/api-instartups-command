using Instartups.Command.Api.DTOs;
using Instartups.Command.Application.Common.Response;

namespace Instartups.Command.Api.UnitTests.Dtos;

public class ResponseDTOTests
{
    [Fact]
    public void Success_DeveCriarRespostaDeSucessoComMensagemEDados()
    {
        var dados = new IdResponse(Guid.NewGuid());
        var antes = DateTimeOffset.UtcNow;

        var resposta = ResponseDTO<IdResponse>.Success("Criado.", dados);

        resposta.IsSuccess.ShouldBeTrue();
        resposta.Message.ShouldBe("Criado.");
        resposta.Data.ShouldBe(dados);
        resposta.DateTimeResponse.ShouldBeInRange(antes, DateTimeOffset.UtcNow);
        resposta.DateTimeResponse.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Fact]
    public void Success_QuandoSemDados_DeveCriarRespostaComDataPadrao()
    {
        var resposta = ResponseDTO<string>.Success("Ok.");

        resposta.IsSuccess.ShouldBeTrue();
        resposta.Data.ShouldBeNull();
    }

    [Fact]
    public void Error_DeveCriarRespostaDeErroComMensagemEDados()
    {
        var erros = new List<ValidationErrorDTO> { new("Nome", "CAMPO_OBRIGATORIO", "O campo é obrigatório.") };
        var antes = DateTimeOffset.UtcNow;

        var resposta = ResponseDTO<IEnumerable<ValidationErrorDTO>>.Error("Erro de validação.", erros);

        resposta.IsSuccess.ShouldBeFalse();
        resposta.Message.ShouldBe("Erro de validação.");
        resposta.Data.ShouldBe(erros);
        resposta.DateTimeResponse.ShouldBeInRange(antes, DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Error_QuandoSemDados_DeveCriarRespostaComDataPadrao()
    {
        var resposta = ResponseDTO<string>.Error("Falhou.");

        resposta.IsSuccess.ShouldBeFalse();
        resposta.Data.ShouldBeNull();
    }
}
