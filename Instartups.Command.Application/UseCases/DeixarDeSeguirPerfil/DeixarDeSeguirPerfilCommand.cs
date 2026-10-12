using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.DeixarDeSeguirPerfil;

public sealed record DeixarDeSeguirPerfilCommand(
    Guid SeguidoId,
    Guid UserId
) : ICommand;
