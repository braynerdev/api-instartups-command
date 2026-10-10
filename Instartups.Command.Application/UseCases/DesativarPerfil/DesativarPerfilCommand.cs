using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.DesativarPerfil;

public sealed record DesativarPerfilCommand(
    Guid UserId
) : ICommand;
