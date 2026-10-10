using FluentValidation;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.Common.Validator;

public class CadastrarPerfilCommandValidator : AbstractValidator<CadastrarPerfilCommand>
{
    public CadastrarPerfilCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
                .WithErrorCode(CodigosErroConst.CampoObrigatorio)
                .WithMessage(MensagensErroConst.CampoObrigatorio)
            .MaximumLength(TamanhosColunasConst.Perfil.Nome)
                .WithErrorCode(CodigosErroConst.TamanhoMaximo)
                .WithMessage(MensagensErroConst.TamanhoMaximo);

        RuleFor(x => x.ImagemPerfilUrl)
            .MaximumLength(TamanhosColunasConst.Perfil.ImagemPerfilUrl)
                .WithErrorCode(CodigosErroConst.TamanhoMaximo)
                .WithMessage(MensagensErroConst.TamanhoMaximo)
            .Must(ValidacaoUrl.EhValida)
                .WithErrorCode(CodigosErroConst.UrlInvalida)
                .WithMessage(MensagensErroConst.UrlInvalida)
            .When(x => x.ImagemPerfilUrl is not null);

        RuleFor(x => x.ImagemFundoUrl)
            .MaximumLength(TamanhosColunasConst.Perfil.ImagemFundoUrl)
                .WithErrorCode(CodigosErroConst.TamanhoMaximo)
                .WithMessage(MensagensErroConst.TamanhoMaximo)
            .Must(ValidacaoUrl.EhValida)
                .WithErrorCode(CodigosErroConst.UrlInvalida)
                .WithMessage(MensagensErroConst.UrlInvalida)
            .When(x => x.ImagemFundoUrl is not null);

        RuleFor(x => x.Coordenada)
            .SetValidator(new CoordenadaCommandValidator());
    }
}