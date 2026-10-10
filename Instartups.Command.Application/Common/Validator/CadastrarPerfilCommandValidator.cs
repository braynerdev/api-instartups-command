using FluentValidation;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Common.Validator.Extensions;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.Common.Validator;

public class CadastrarPerfilCommandValidator : AbstractValidator<CadastrarPerfilCommand>
{
    public CadastrarPerfilCommandValidator()
    {
        RuleFor(x => x.Nome)
            .ValidarNome();

        RuleFor(x => x.ImagemPerfilUrl)
            .ValidarImagemUrl(TamanhosColunasConst.Perfil.ImagemPerfilUrl)
            .When(x => x.ImagemPerfilUrl is not null);

        RuleFor(x => x.ImagemFundoUrl)
            .ValidarImagemUrl(TamanhosColunasConst.Perfil.ImagemFundoUrl)
            .When(x => x.ImagemFundoUrl is not null);

        RuleFor(x => x.Coordenada)
            .SetValidator(new CoordenadaCommandValidator());
    }
}
