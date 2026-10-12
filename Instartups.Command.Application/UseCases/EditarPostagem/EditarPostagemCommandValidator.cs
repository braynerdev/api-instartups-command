using FluentValidation;
using Instartups.Command.Application.Common.Validator.Extensions;

namespace Instartups.Command.Application.UseCases.EditarPostagem;

public class EditarPostagemCommandValidator : AbstractValidator<EditarPostagemCommand>
{
    public EditarPostagemCommandValidator()
    {
        RuleFor(x => x.PostagemId)
            .ValidarId();

        RuleFor(x => x.Descricao)
            .ValidarDescricaoPostagem();

        RuleFor(x => x.UserId)
            .ValidarId();

        RuleFor(x => x.MidiasRemovidas)
            .ForEach(midiaId => midiaId
                .ValidarId()
            );

        RuleFor(x => x.MidiasAdicionadas)
            .ForEach(midia => midia
                .ValidarMidia()
            );
    }
}
