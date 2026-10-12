using FluentValidation;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.Common.Validator.Extensions;

public static class PitchExtensions
{
    public static IRuleBuilderOptions<T, string> ValidarPitch<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .MaximumLength(TamanhosColunasConst.Startup.Pitch)
                .WithErrorCode(CodigosErroConst.TamanhoMaximo)
                .WithMessage(MensagensErroConst.TamanhoMaximo);
    }
}
