using FluentValidation;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Instartups.Command.Application.Common.Validator;

public class CoordenadaCommandValidator : AbstractValidator<CoordenadaCommand>
{
    public CoordenadaCommandValidator()
    {
        RuleFor(x => x.Latitude)
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .InclusiveBetween(-90, 90)
                .WithErrorCode(CodigosErroConst.ValorEntre)
                .WithMessage(MensagensErroConst.ValorEntre);
        RuleFor(x => x.Longitude)
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .InclusiveBetween(-180, 180)
                .WithErrorCode(CodigosErroConst.ValorEntre)
                .WithMessage(MensagensErroConst.ValorEntre);
    }
}
