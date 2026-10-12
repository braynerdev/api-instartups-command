using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.EditarCoordenadaPerfil;

public sealed record EditarCoordenadaPerfilCommand(
    CoordenadaCommand Coordenada,
    Guid UserId
) : ICommand;
