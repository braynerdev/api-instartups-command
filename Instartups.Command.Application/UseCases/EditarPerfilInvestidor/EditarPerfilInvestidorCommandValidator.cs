using FluentValidation;
using Instartups.Command.Application.Common.Validator;
using Instartups.Command.Application.Common.Validator.Extensions;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UseCases.EditarPerfilInvestidor;

public class EditarPerfilInvestidorCommandValidator : AbstractValidator<EditarPerfilInvestidorCommand>
{
    public EditarPerfilInvestidorCommandValidator()
    {
        RuleFor(x => x.Perfil)
            .NotNull()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .SetValidator(new EditarPerfilCommandValidator());

        RuleFor(x => x.TeseInvestimento)
            .ValidarTeseInvestimento();

        RuleFor(x => x.TicketMinimo)
            .ValidarTicketMinimo();

        RuleFor(x => x.TicketMaximo)
            .ValidarTicketMaximo(x => x.TicketMinimo);
    }
}
