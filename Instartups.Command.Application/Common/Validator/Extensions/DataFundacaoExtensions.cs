using FluentValidation;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.Common.Validator.Extensions;

public static class DataFundacaoExtensions
{
    public static IRuleBuilderOptions<T, DateOnly> ValidarDataFundacao<T>(
        this IRuleBuilder<T, DateOnly> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.UtcNow))
                .WithErrorCode(CodigosErroConst.DataFutura)
                .WithMessage(MensagensErroConst.DataFutura);
    }
}
