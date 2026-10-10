using FluentValidation;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Common.Validator.Extensions;

namespace Instartups.Command.Application.Common.Validator;

public class CoordenadaCommandValidator : AbstractValidator<CoordenadaCommand>
{
    public CoordenadaCommandValidator()
    {
        RuleFor(x => x.Latitude)
            .ValidarLatitude();

        RuleFor(x => x.Longitude)
            .ValidarLongitude();
    }
}
