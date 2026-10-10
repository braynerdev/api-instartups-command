using FluentValidation;
using Instartups.Command.Application.Common.Validator.Extensions;

namespace Instartups.Command.Application.UseCases.DeixarDeSeguirPerfil;

public class DeixarDeSeguirPerfilCommandValidator : AbstractValidator<DeixarDeSeguirPerfilCommand>
{
    public DeixarDeSeguirPerfilCommandValidator()
    {
        RuleFor(x => x.SeguidoId)
            .ValidarSeguidoId();
    }
}
