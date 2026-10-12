using FluentValidation;
using Instartups.Command.Application.Common.Validator.Extensions;

namespace Instartups.Command.Application.UseCases.CadastrarPostagem;

public class CadastrarPostagemCommandValidator : AbstractValidator<CadastrarPostagemCommand>
{
    public CadastrarPostagemCommandValidator()
    {
        RuleFor(x => x.Descricao)
            .ValidarDescricaoPostagem();
        
        RuleFor(x => x.AutorId)
            .ValidarId();

        RuleFor(x => x.Midias)
            .ForEach(midia => midia
                .ValidarMidia()
            );
    }
}