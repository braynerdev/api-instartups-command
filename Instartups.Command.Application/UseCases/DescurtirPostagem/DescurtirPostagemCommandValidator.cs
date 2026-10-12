using FluentValidation;
using Instartups.Command.Application.Common.Validator.Extensions;

namespace Instartups.Command.Application.UseCases.DescurtirPostagem;

public class DescurtirPostagemCommandValidator : AbstractValidator<DescurtirPostagemCommand>
{
    public DescurtirPostagemCommandValidator()
    {
        RuleFor(x => x.PostagemId)
            .ValidarId();
    }
}
