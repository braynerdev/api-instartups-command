using FluentValidation.TestHelper;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Common.Validator;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UnitTests.Common.Validators;

public class CoordenadaCommandValidatorTests
{
    private readonly CoordenadaCommandValidator _validator = new();

    [Theory]
    [InlineData(-23.5505, -46.6333)]
    [InlineData(-90, -180)]
    [InlineData(90, 180)]
    [InlineData(0.0001, -0.0001)]
    public void Validar_QuandoCoordenadaDentroDosLimites_NaoDeveRetornarErros(double latitude, double longitude)
    {
        var resultado = _validator.TestValidate(new CoordenadaCommand(latitude, longitude));

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(-90.0001)]
    [InlineData(90.0001)]
    [InlineData(-180)]
    [InlineData(180)]
    public void Validar_QuandoLatitudeForaDoIntervalo_DeveRetornarErroValorEntre(double latitude)
    {
        var resultado = _validator.TestValidate(new CoordenadaCommand(latitude, 10));

        resultado.ShouldHaveValidationErrorFor(x => x.Latitude)
            .WithErrorCode(CodigosErroConst.ValorEntre)
            .WithErrorMessage("O valor do campo deve estar entre -90 e 90.");
        resultado.ShouldNotHaveValidationErrorFor(x => x.Longitude);
    }

    [Theory]
    [InlineData(-180.0001)]
    [InlineData(180.0001)]
    [InlineData(360)]
    public void Validar_QuandoLongitudeForaDoIntervalo_DeveRetornarErroValorEntre(double longitude)
    {
        var resultado = _validator.TestValidate(new CoordenadaCommand(10, longitude));

        resultado.ShouldHaveValidationErrorFor(x => x.Longitude)
            .WithErrorCode(CodigosErroConst.ValorEntre)
            .WithErrorMessage("O valor do campo deve estar entre -180 e 180.");
        resultado.ShouldNotHaveValidationErrorFor(x => x.Latitude);
    }

    [Fact]
    public void Validar_QuandoLatitudeZero_DeveRetornarErroCampoObrigatorio()
    {
        var resultado = _validator.TestValidate(new CoordenadaCommand(0, 10));

        var erro = resultado.Errors.ShouldHaveSingleItem();
        erro.PropertyName.ShouldBe(nameof(CoordenadaCommand.Latitude));
        erro.ErrorCode.ShouldBe(CodigosErroConst.CampoObrigatorio);
        erro.ErrorMessage.ShouldBe(MensagensErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoLongitudeZero_DeveRetornarErroCampoObrigatorio()
    {
        var resultado = _validator.TestValidate(new CoordenadaCommand(10, 0));

        var erro = resultado.Errors.ShouldHaveSingleItem();
        erro.PropertyName.ShouldBe(nameof(CoordenadaCommand.Longitude));
        erro.ErrorCode.ShouldBe(CodigosErroConst.CampoObrigatorio);
    }
}
