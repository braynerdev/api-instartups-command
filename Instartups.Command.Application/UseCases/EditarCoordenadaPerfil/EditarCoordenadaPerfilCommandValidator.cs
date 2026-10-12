using FluentValidation;
using Instartups.Command.Application.Common.Validator;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UseCases.EditarCoordenadaPerfil;

public class EditarCoordenadaPerfilCommandValidator : AbstractValidator<EditarCoordenadaPerfilCommand>
{
    public EditarCoordenadaPerfilCommandValidator()
    {
        RuleFor(x => x.Coordenada)
            .NotNull()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .SetValidator(new CoordenadaCommandValidator());
    }
}
