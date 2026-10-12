using FluentValidation.TestHelper;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.UseCases.EditarCoordenadaPerfil;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Tests.Common.Builders.Commands;

namespace Instartups.Command.Application.UnitTests.UseCases.EditarCoordenadaPerfil;

public class EditarCoordenadaPerfilCommandValidatorTests
{
    private readonly EditarCoordenadaPerfilCommandValidator _validator = new();

    [Fact]
    public void Validar_QuandoCoordenadaValida_NaoDeveRetornarErros()
    {
        _validator.TestValidate(PerfilCommandBuilder.EditarCoordenada()).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_QuandoLatitudeForaDoIntervalo_DeveRetornarErroNaPropriedadeAninhada()
    {
        var command = PerfilCommandBuilder.EditarCoordenada() with { Coordenada = new CoordenadaCommand(-90.5, 10) };

        _validator.TestValidate(command)
            .ShouldHaveValidationErrorFor("Coordenada.Latitude")
            .WithErrorCode(CodigosErroConst.ValorEntre);
    }

    [Fact]
    public void Validar_QuandoLongitudeForaDoIntervalo_DeveRetornarErroNaPropriedadeAninhada()
    {
        var command = PerfilCommandBuilder.EditarCoordenada() with { Coordenada = new CoordenadaCommand(10, 180.5) };

        _validator.TestValidate(command)
            .ShouldHaveValidationErrorFor("Coordenada.Longitude")
            .WithErrorCode(CodigosErroConst.ValorEntre);
    }

    [Fact]
    public void Validar_QuandoCoordenadaZerada_DeveRetornarErroCampoObrigatorioEmAmbas()
    {
        var command = PerfilCommandBuilder.EditarCoordenada() with { Coordenada = new CoordenadaCommand(0, 0) };

        var resultado = _validator.TestValidate(command);

        resultado.ShouldHaveValidationErrorFor("Coordenada.Latitude").WithErrorCode(CodigosErroConst.CampoObrigatorio);
        resultado.ShouldHaveValidationErrorFor("Coordenada.Longitude").WithErrorCode(CodigosErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoCoordenadaNula_DeveRetornarErroCampoObrigatorio()
    {
        var command = PerfilCommandBuilder.EditarCoordenada() with { Coordenada = null! };

        _validator.TestValidate(command)
            .ShouldHaveValidationErrorFor("Coordenada")
            .WithErrorCode(CodigosErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoUserIdVazio_NaoDeveRetornarErros()
    {
        var command = PerfilCommandBuilder.EditarCoordenada() with { UserId = Guid.Empty };

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}
