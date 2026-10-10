using FluentValidation;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.Common.Validator.Extensions;

public static class ValorBuscadoExtensions
{
    public static IRuleBuilderOptions<T, decimal> ValidarValorBuscado<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .GreaterThan(0)
                .WithErrorCode(CodigosErroConst.ValorMinimo)
                .WithMessage(MensagensErroConst.ValorMinimo)
            .PrecisionScale(TamanhosColunasConst.Startup.ValorBuscadoPrecisao, TamanhosColunasConst.Startup.ValorBuscadoEscala, true)
                .WithErrorCode(CodigosErroConst.PrecisaoIncorreta)
                .WithMessage(MensagensErroConst.PrecisaoIncorreta);
    }
}
