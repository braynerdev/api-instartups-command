using System.Linq.Expressions;
using FluentValidation;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.Common.Validator.Extensions;

public static class TicketExtensions
{
    public static IRuleBuilderOptions<T, decimal> ValidarTicketMinimo<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .GreaterThan(0)
                .WithErrorCode(CodigosErroConst.ValorMinimo)
                .WithMessage(MensagensErroConst.ValorMinimo)
            .PrecisionScale(TamanhosColunasConst.Investidor.TicketPrecisao, TamanhosColunasConst.Investidor.TicketEscala, true)
                .WithErrorCode(CodigosErroConst.PrecisaoIncorreta)
                .WithMessage(MensagensErroConst.PrecisaoIncorreta);
    }

    public static IRuleBuilderOptions<T, decimal> ValidarTicketMaximo<T>(
        this IRuleBuilder<T, decimal> ruleBuilder,
        Expression<Func<T, decimal>> ticketMinimo)
    {
        return ruleBuilder
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .GreaterThan(ticketMinimo)
                .WithErrorCode(CodigosErroConst.ValorMinimo)
                .WithMessage(MensagensErroConst.ValorMinimo)
            .PrecisionScale(TamanhosColunasConst.Investidor.TicketPrecisao, TamanhosColunasConst.Investidor.TicketEscala, true)
                .WithErrorCode(CodigosErroConst.PrecisaoIncorreta)
                .WithMessage(MensagensErroConst.PrecisaoIncorreta);
    }
}
