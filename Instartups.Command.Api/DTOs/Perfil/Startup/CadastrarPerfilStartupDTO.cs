using Instartups.Command.Application.UseCases.CadastrarPerfilStartup;

namespace Instartups.Command.Api.DTOs.Perfil.Startup;

public sealed record CadastrarPerfilStartupDTO(
    CadastrarPerfilDTO Perfil,
    string Pitch,
    DateOnly DataFundacao,
    int TamanhoEquipe,
    decimal ValorBuscado
)
{
    public CadastrarPerfilStartupCommand ToCommand(Guid userId)
    {
        return new CadastrarPerfilStartupCommand(
            Perfil.ToCommand(userId),
            Pitch,
            DataFundacao,
            TamanhoEquipe,
            ValorBuscado
        );
    }
}
