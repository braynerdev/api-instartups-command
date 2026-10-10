using FluentValidation;
using Instartups.Command.Application.Common.Validator;
using Instartups.Command.Application.Common.Validator.Extensions;

namespace Instartups.Command.Application.UseCases.EditarPerfilInvestidor;

public class EditarPerfilInvestidorCommandValidator : AbstractValidator<EditarPerfilInvestidorCommand>
{
    public EditarPerfilInvestidorCommandValidator()
    {
        RuleFor(x => x.Perfil)
            .SetValidator(new EditarPerfilCommandValidator());

        RuleFor(x => x.TeseInvestimento)
            .ValidarTeseInvestimento();

        RuleFor(x => x.TicketMinimo)
            .ValidarTicketMinimo();

        RuleFor(x => x.TicketMaximo)
            .ValidarTicketMaximo(x => x.TicketMinimo);
    }
}
