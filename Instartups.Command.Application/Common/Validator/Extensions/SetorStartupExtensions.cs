using FluentValidation;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Enums;

namespace Instartups.Command.Application.Common.Validator.Extensions;

public static class SetorStartupExtensions
{
    public static IRuleBuilderOptions<T, SetorStartupEnum> ValidarSetor<T>(
        this IRuleBuilder<T, SetorStartupEnum> ruleBuilder)
    {
        return ruleBuilder
            .IsInEnum()
                .WithErrorCode(CodigosErroConst.ValorInvalidoEnum)
                .WithMessage(MensagensErroConst.ValorInvalidoEnum);
    }
}
