using FluentValidation;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.Common.Validator.Extensions;

public static class ImagemUrlExtensions
{
    public static IRuleBuilderOptions<T, string?> ValidarImagemUrl<T>(
        this IRuleBuilder<T, string?> ruleBuilder, int tamanhoMaximo)
    {
        return ruleBuilder
            .MaximumLength(tamanhoMaximo)
                .WithErrorCode(CodigosErroConst.TamanhoMaximo)
                .WithMessage(MensagensErroConst.TamanhoMaximo)
            .Must(ValidarUrl.Valida)
                .WithErrorCode(CodigosErroConst.UrlInvalida)
                .WithMessage(MensagensErroConst.UrlInvalida);
    }
}
