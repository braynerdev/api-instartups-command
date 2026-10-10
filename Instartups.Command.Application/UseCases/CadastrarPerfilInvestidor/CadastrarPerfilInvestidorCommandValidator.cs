using FluentValidation;
using Instartups.Command.Application.Common.Validator;
using Instartups.Command.Application.Common.Validator.Extensions;

namespace Instartups.Command.Application.UseCases.CadastrarPerfilInvestidor;

public class CadastrarPerfilInvestidorCommandValidator : AbstractValidator<CadastrarPerfilInvestidorCommand>
{
    public CadastrarPerfilInvestidorCommandValidator()
    {
        RuleFor(x => x.Perfil)
            .SetValidator(new CadastrarPerfilCommandValidator());

        RuleFor(x => x.TeseInvestimento)
            .ValidarTeseInvestimento();

        RuleFor(x => x.TicketMinimo)
            .ValidarTicketMinimo();

        RuleFor(x => x.TicketMaximo)
            .ValidarTicketMaximo(x => x.TicketMinimo);
    }
}
