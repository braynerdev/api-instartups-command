using FluentValidation;
using Instartups.Command.Application.Common.Validator.Extensions;

namespace Instartups.Command.Application.UseCases.SeguirPerfil;

public class SeguirPerfilCommandValidator : AbstractValidator<SeguirPerfilCommand>
{
    public SeguirPerfilCommandValidator()
    {
        RuleFor(x => x.SeguidoId)
            .ValidarSeguidoId();
    }
}
