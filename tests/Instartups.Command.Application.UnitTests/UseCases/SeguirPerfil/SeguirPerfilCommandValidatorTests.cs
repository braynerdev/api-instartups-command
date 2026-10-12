using FluentValidation.TestHelper;
using Instartups.Command.Application.UseCases.SeguirPerfil;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UnitTests.UseCases.SeguirPerfil;

public class SeguirPerfilCommandValidatorTests
{
    private readonly SeguirPerfilCommandValidator _validator = new();

    [Fact]
    public void Validar_QuandoIdsPreenchidos_NaoDeveRetornarErros()
    {
        var command = new SeguirPerfilCommand(Guid.NewGuid(), Guid.NewGuid());

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_QuandoSeguidoIdVazio_DeveRetornarErroCampoObrigatorio()
    {
        var command = new SeguirPerfilCommand(Guid.Empty, Guid.NewGuid());

        _validator.TestValidate(command)
            .ShouldHaveValidationErrorFor(x => x.SeguidoId)
            .WithErrorCode(CodigosErroConst.CampoObrigatorio)
            .WithErrorMessage(MensagensErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoUserIdVazio_NaoDeveRetornarErros()
    {
        var command = new SeguirPerfilCommand(Guid.NewGuid(), Guid.Empty);

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}
