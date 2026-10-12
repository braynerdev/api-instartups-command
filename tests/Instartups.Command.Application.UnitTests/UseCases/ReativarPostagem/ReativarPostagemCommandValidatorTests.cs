using FluentValidation.TestHelper;
using Instartups.Command.Application.UseCases.ReativarPostagem;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UnitTests.UseCases.ReativarPostagem;

public class ReativarPostagemCommandValidatorTests
{
    private readonly ReativarPostagemCommandValidator _validator = new();

    [Fact]
    public void Validar_QuandoIdsPreenchidos_NaoDeveRetornarErros()
    {
        var command = new ReativarPostagemCommand(Guid.NewGuid(), Guid.NewGuid());

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_QuandoPostagemIdVazio_DeveRetornarErroCampoObrigatorio()
    {
        var command = new ReativarPostagemCommand(Guid.Empty, Guid.NewGuid());

        _validator.TestValidate(command)
            .ShouldHaveValidationErrorFor(x => x.PostagemId)
            .WithErrorCode(CodigosErroConst.CampoObrigatorio)
            .WithErrorMessage(MensagensErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoUserIdVazio_NaoDeveRetornarErros()
    {
        var command = new ReativarPostagemCommand(Guid.NewGuid(), Guid.Empty);

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}
