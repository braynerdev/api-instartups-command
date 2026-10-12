using FluentValidation;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.Common.Validator.Extensions;

public static class DescricaoPostagemExtensions
{
    public static IRuleBuilderOptions<T, string> ValidarDescricaoPostagem<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .MaximumLength(TamanhosColunasConst.Postagem.Descricao)
                .WithErrorCode(CodigosErroConst.TamanhoMaximo)
                .WithMessage(MensagensErroConst.TamanhoMaximo);
    }
}