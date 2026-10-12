using FluentValidation;
using Instartups.Command.Application.Common.Validator.Extensions;

namespace Instartups.Command.Application.UseCases.ReativarPostagem;

public class ReativarPostagemCommandValidator : AbstractValidator<ReativarPostagemCommand>
{
    public ReativarPostagemCommandValidator()
    {
        RuleFor(x => x.PostagemId)
            .ValidarId();
    }
}
