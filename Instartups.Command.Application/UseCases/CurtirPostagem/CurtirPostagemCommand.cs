using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.CurtirPostagem;

public sealed record CurtirPostagemCommand(
    Guid PostagemId,
    Guid UserId
) : ICommand;
