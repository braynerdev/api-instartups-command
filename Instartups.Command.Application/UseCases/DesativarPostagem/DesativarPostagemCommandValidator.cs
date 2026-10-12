using FluentValidation;
using Instartups.Command.Application.Common.Validator.Extensions;

namespace Instartups.Command.Application.UseCases.DesativarPostagem;

public class DesativarPostagemCommandValidator : AbstractValidator<DesativarPostagemCommand>
{
    public DesativarPostagemCommandValidator()
    {
        RuleFor(x => x.PostagemId)
            .ValidarId();
    }
}
