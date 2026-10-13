using FluentValidation;
using Instartups.Command.Application.Common.Validator;
using Instartups.Command.Application.Common.Validator.Extensions;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UseCases.EditarPerfilStartup;

public class EditarPerfilStartupCommandValidator : AbstractValidator<EditarPerfilStartupCommand>
{
    public EditarPerfilStartupCommandValidator()
    {
        RuleFor(x => x.Perfil)
            .NotNull()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .SetValidator(new EditarPerfilCommandValidator());

        RuleFor(x => x.Pitch)
            .ValidarPitch();

        RuleFor(x => x.DataFundacao)
            .ValidarDataFundacao();

        RuleFor(x => x.TamanhoEquipe)
            .ValidarTamanhoEquipe();

        RuleFor(x => x.ValorBuscado)
            .ValidarValorBuscado();

        RuleFor(x => x.Setor)
            .ValidarSetor();
    }
}
