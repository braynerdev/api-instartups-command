using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.ReativarPostagem;

public sealed record ReativarPostagemCommand(
    Guid PostagemId,
    Guid UserId
) : ICommand;
