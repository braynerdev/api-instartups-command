using Instartups.Command.Application.UseCases.CadastrarPerfilInvestidor;

namespace Instartups.Command.Api.DTOs.Perfil.Investidor;

public sealed record CadastrarPerfilInvestidorDTO(
    CadastrarPerfilDTO Perfil,
    string TeseInvestimento,
    decimal TicketMinimo,
    decimal TicketMaximo
)
{
    public CadastrarPerfilInvestidorCommand ToCommand(Guid userId)
    {
        return new CadastrarPerfilInvestidorCommand(
            Perfil.ToCommand(userId),
            TeseInvestimento,
            TicketMinimo,
            TicketMaximo
        );
    }
}
