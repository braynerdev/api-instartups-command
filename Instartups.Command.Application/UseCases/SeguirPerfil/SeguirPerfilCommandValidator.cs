using FluentValidation;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UseCases.SeguirPerfil;

public class SeguirPerfilCommandValidator : AbstractValidator<SeguirPerfilCommand>
{
    public SeguirPerfilCommandValidator()
    {
        RuleFor(x => x.SeguidoId)
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio);
    }
}
