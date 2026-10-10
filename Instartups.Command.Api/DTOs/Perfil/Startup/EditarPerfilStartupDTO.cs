using Instartups.Command.Application.UseCases.EditarPerfilStartup;

namespace Instartups.Command.Api.DTOs.Perfil.Startup;

public sealed record EditarPerfilStartupDTO(
    EditarPerfilDTO Perfil,
    string Pitch,
    DateOnly DataFundacao,
    int TamanhoEquipe,
    decimal ValorBuscado
)
{
    public EditarPerfilStartupCommand ToCommand(Guid userId)
    {
        return new EditarPerfilStartupCommand(
            Perfil.ToCommand(userId),
            Pitch,
            DataFundacao,
            TamanhoEquipe,
            ValorBuscado
        );
    }
}
