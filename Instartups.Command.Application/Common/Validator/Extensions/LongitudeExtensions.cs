using FluentValidation;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.Common.Validator.Extensions;

public static class LongitudeExtensions
{
    public static IRuleBuilderOptions<T, double> ValidarLongitude<T>(
        this IRuleBuilder<T, double> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .InclusiveBetween(-180, 180)
                .WithErrorCode(CodigosErroConst.ValorEntre)
                .WithMessage(MensagensErroConst.ValorEntre);
    }
}
