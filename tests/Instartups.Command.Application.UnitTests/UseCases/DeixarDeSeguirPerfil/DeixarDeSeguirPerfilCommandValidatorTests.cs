using FluentValidation.TestHelper;
using Instartups.Command.Application.UseCases.DeixarDeSeguirPerfil;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UnitTests.UseCases.DeixarDeSeguirPerfil;

public class DeixarDeSeguirPerfilCommandValidatorTests
{
    private readonly DeixarDeSeguirPerfilCommandValidator _validator = new();

    [Fact]
    public void Validar_QuandoIdsPreenchidos_NaoDeveRetornarErros()
    {
        var command = new DeixarDeSeguirPerfilCommand(Guid.NewGuid(), Guid.NewGuid());

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_QuandoSeguidoIdVazio_DeveRetornarErroCampoObrigatorio()
    {
        var command = new DeixarDeSeguirPerfilCommand(Guid.Empty, Guid.NewGuid());

        _validator.TestValidate(command)
            .ShouldHaveValidationErrorFor(x => x.SeguidoId)
            .WithErrorCode(CodigosErroConst.CampoObrigatorio)
            .WithErrorMessage(MensagensErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoUserIdVazio_NaoDeveRetornarErros()
    {
        var command = new DeixarDeSeguirPerfilCommand(Guid.NewGuid(), Guid.Empty);

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}
