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
            // Compara com a data de Brasília: com UtcNow, das 21h às 23h59 a data de amanhã passava.
            .LessThanOrEqualTo(_ => DataReferencia.HojeBrasilia())
                .WithErrorCode(CodigosErroConst.DataFutura)
                .WithMessage(MensagensErroConst.DataFutura);
    }
}
