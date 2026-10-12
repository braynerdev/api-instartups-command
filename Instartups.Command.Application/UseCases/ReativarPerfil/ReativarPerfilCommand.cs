using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.ReativarPerfil;

public sealed record ReativarPerfilCommand(
    Guid UserId
) : ICommand;
