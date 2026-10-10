using FluentValidation;
using Instartups.Command.Application.Common.Validator;
using Instartups.Command.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Instartups.Command.Application.UseCases.CadastrarPerfilInvestidor;

public class CadastrarPerfilInvestidorCommandValidator : AbstractValidator<CadastrarPerfilInvestidorCommand>
{
    public CadastrarPerfilInvestidorCommandValidator()
    {
        RuleFor(x => x.Perfil)
            .SetValidator(new CadastrarPerfilCommandValidator());

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


    //string TeseInvestimento,
    //decimal TicketMinimo,
    //decimal TicketMaximo