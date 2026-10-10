using FluentValidation;
using Instartups.Command.Application.Common.Validator;
using Instartups.Command.Application.Common.Validator.Extensions;

namespace Instartups.Command.Application.UseCases.CadastrarPerfilStartup;

public class CadastrarPerfilStartupCommandValidator : AbstractValidator<CadastrarPerfilStartupCommand>
{
    public CadastrarPerfilStartupCommandValidator()
    {
        RuleFor(x => x.Perfil)
            .SetValidator(new CadastrarPerfilCommandValidator());

        RuleFor(x => x.Pitch)
            .ValidarPitch();

        RuleFor(x => x.DataFundacao)
            .ValidarDataFundacao();

        RuleFor(x => x.TamanhoEquipe)
            .ValidarTamanhoEquipe();

        RuleFor(x => x.ValorBuscado)
            .ValidarValorBuscado();
    }
}
