using FluentValidation;
using Instartups.Command.Application.Common.Validator.Extensions;

namespace Instartups.Command.Application.UseCases.CurtirPostagem;

public class CurtirPostagemCommandValidator : AbstractValidator<CurtirPostagemCommand>
{
    public CurtirPostagemCommandValidator()
    {
        RuleFor(x => x.PostagemId)
            .ValidarId();
    }
}
