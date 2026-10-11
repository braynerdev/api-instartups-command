using FluentValidation;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.Common.Validator.Extensions;

public static class ValidarMidiaPostagemExtensions
{
    public static IRuleBuilderOptions<T, MidiaCommand> ValidarMidia<T>(
        this IRuleBuilder<T, MidiaCommand> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .ChildRules(midia =>
            {
                midia.RuleFor(m => m.Url)
                    .NotEmpty()
                        .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                        .WithMessage(MensagensErroConst.CampoObrigatorio)
                    .MaximumLength(TamanhosColunasConst.MidiaPostagem.Url)
                        .WithErrorCode(CodigosErroConst.TamanhoMaximo)
                        .WithMessage(MensagensErroConst.TamanhoMaximo)
                    .Must(ValidarUrl.Valida)
                        .WithErrorCode(CodigosErroConst.UrlInvalida)
                        .WithMessage(MensagensErroConst.UrlInvalida);

                midia.RuleFor(m => m.Tipo)
                    .IsInEnum()
                        .WithErrorCode(CodigosErroConst.ValorInvalidoEnum)
                        .WithMessage(MensagensErroConst.ValorInvalidoEnum);
            });
    }
}