using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.SeguirPerfil;

public sealed record SeguirPerfilCommand(
    Guid SeguidoId,
    Guid UserId
) : ICommand;
