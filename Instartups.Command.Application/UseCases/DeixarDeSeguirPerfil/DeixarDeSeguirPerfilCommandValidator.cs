using FluentValidation;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UseCases.DeixarDeSeguirPerfil;

public class DeixarDeSeguirPerfilCommandValidator : AbstractValidator<DeixarDeSeguirPerfilCommand>
{
    public DeixarDeSeguirPerfilCommandValidator()
    {
        RuleFor(x => x.SeguidoId)
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio);
    }
}
