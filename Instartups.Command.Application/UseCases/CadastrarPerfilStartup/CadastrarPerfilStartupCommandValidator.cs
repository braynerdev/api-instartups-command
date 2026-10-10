using FluentValidation;
using Instartups.Command.Application.Common.Validator;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UseCases.CadastrarPerfilStartup;

public class CadastrarPerfilStartupCommandValidator : AbstractValidator<CadastrarPerfilStartupCommand>
{
    public CadastrarPerfilStartupCommandValidator()
    {
        RuleFor(x => x.Perfil)
            .SetValidator(new CadastrarPerfilCommandValidator());

        RuleFor(x => x.Pitch)
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .MaximumLength(TamanhosColunasConst.Startup.Pitch)
                .WithErrorCode(CodigosErroConst.TamanhoMaximo)
                .WithMessage(MensagensErroConst.TamanhoMaximo);

        RuleFor(x => x.DataFundacao)
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.UtcNow))
                .WithErrorCode(CodigosErroConst.DataFutura)
                .WithMessage(MensagensErroConst.DataFutura);

        RuleFor(x => x.TamanhoEquipe)
            .InclusiveBetween(1, short.MaxValue)
                .WithErrorCode(CodigosErroConst.ValorEntre)
                .WithMessage(MensagensErroConst.ValorEntre);

        RuleFor(x => x.ValorBuscado)
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .GreaterThan(0)
                .WithErrorCode(CodigosErroConst.ValorMinimo)
                .WithMessage(MensagensErroConst.ValorMinimo)
            .PrecisionScale(TamanhosColunasConst.Startup.ValorBuscadoPrecisao, TamanhosColunasConst.Startup.ValorBuscadoEscala, true)
                .WithErrorCode(CodigosErroConst.PrecisaoIncorreta)
                .WithMessage(MensagensErroConst.PrecisaoIncorreta);
    }
}
