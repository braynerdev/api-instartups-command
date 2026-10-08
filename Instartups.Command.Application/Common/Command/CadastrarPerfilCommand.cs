namespace Instartups.Command.Application.Common.Command;

public sealed record CadastrarPerfilCommand(
    string Nome,
    string? ImagemPerfilUrl,
    string? ImagemFundoUrl,
    CoordenadaCommand Coordenada,
    Guid UserId
);
