using FluentValidation;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.Common.Validator.Extensions;

public static class LatitudeExtensions
{
    public static IRuleBuilderOptions<T, double> ValidarLatitude<T>(
        this IRuleBuilder<T, double> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .InclusiveBetween(-90, 90)
                .WithErrorCode(CodigosErroConst.ValorEntre)
                .WithMessage(MensagensErroConst.ValorEntre);
    }
}
