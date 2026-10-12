namespace Instartups.Command.Application.Common.Command;

public sealed record EditarPerfilCommand
(
    string Nome,
    string? ImagemPerfilUrl,
    string? ImagemFundoUrl,
    CoordenadaCommand Coordenada,
    Guid UserId
);
