using FluentValidation;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.Common.Validator.Extensions;

public static class TamanhoEquipeExtensions
{
    public static IRuleBuilderOptions<T, int> ValidarTamanhoEquipe<T>(
        this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .InclusiveBetween(1, short.MaxValue)
                .WithErrorCode(CodigosErroConst.ValorEntre)
                .WithMessage(MensagensErroConst.ValorEntre);
    }
}
