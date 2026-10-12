using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.DesativarPostagem;

public sealed record DesativarPostagemCommand(
    Guid PostagemId,
    Guid UserId
) : ICommand;
