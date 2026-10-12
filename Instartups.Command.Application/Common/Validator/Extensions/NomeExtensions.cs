using FluentValidation;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.Common.Validator.Extensions;

public static class NomeExtensions
{
    public static IRuleBuilderOptions<T, string> ValidarNome<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .MaximumLength(TamanhosColunasConst.Perfil.Nome)
                .WithErrorCode(CodigosErroConst.TamanhoMaximo)
                .WithMessage(MensagensErroConst.TamanhoMaximo);
    }
}
