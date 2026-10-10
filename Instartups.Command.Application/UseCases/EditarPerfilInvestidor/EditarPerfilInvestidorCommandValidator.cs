using FluentValidation;
using Instartups.Command.Application.Common.Validator;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UseCases.EditarPerfilInvestidor;

public class EditarPerfilInvestidorCommandValidator : AbstractValidator<EditarPerfilInvestidorCommand>
{
    public EditarPerfilInvestidorCommandValidator()
    {
        RuleFor(x => x.Perfil)
            .SetValidator(new EditarPerfilCommandValidator());

        RuleFor(x => x.TeseInvestimento)
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .MaximumLength(TamanhosColunasConst.Investidor.Tese)
                .WithErrorCode(CodigosErroConst.TamanhoMaximo)
                .WithMessage(MensagensErroConst.TamanhoMaximo);

        RuleFor(x => x.TicketMinimo)
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .GreaterThan(0)
                .WithErrorCode(CodigosErroConst.ValorMinimo)
                .WithMessage(MensagensErroConst.ValorMinimo)
            .PrecisionScale(TamanhosColunasConst.Investidor.TicketPrecisao, TamanhosColunasConst.Investidor.TicketEscala, true)
                .WithErrorCode(CodigosErroConst.PrecisaoIncorreta)
                .WithMessage(MensagensErroConst.PrecisaoIncorreta);

        RuleFor(x => x.TicketMaximo)
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .GreaterThan(x => x.TicketMinimo)
                .WithErrorCode(CodigosErroConst.ValorMinimo)
                .WithMessage(MensagensErroConst.ValorMinimo)
            .PrecisionScale(TamanhosColunasConst.Investidor.TicketPrecisao, TamanhosColunasConst.Investidor.TicketEscala, true)
                .WithErrorCode(CodigosErroConst.PrecisaoIncorreta)
                .WithMessage(MensagensErroConst.PrecisaoIncorreta);
    }
}
