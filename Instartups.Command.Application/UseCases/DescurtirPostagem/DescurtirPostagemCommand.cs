using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.DescurtirPostagem;

public sealed record DescurtirPostagemCommand(
    Guid PostagemId,
    Guid UserId
) : ICommand;
