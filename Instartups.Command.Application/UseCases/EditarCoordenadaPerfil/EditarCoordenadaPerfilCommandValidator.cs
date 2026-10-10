using FluentValidation;
using Instartups.Command.Application.Common.Validator;

namespace Instartups.Command.Application.UseCases.EditarCoordenadaPerfil;

public class EditarCoordenadaPerfilCommandValidator : AbstractValidator<EditarCoordenadaPerfilCommand>
{
    public EditarCoordenadaPerfilCommandValidator()
    {
        RuleFor(x => x.Coordenada)
            .SetValidator(new CoordenadaCommandValidator());
    }
}
