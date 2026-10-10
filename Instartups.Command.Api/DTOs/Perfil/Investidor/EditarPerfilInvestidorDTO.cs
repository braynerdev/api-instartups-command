using Instartups.Command.Application.UseCases.EditarPerfilInvestidor;

namespace Instartups.Command.Api.DTOs.Perfil.Investidor;

public sealed record EditarPerfilInvestidorDTO(
    EditarPerfilDTO Perfil,
    string TeseInvestimento,
    decimal TicketMinimo,
    decimal TicketMaximo
)
{
    public EditarPerfilInvestidorCommand ToCommand(Guid userId)
    {
        return new EditarPerfilInvestidorCommand(
            Perfil.ToCommand(userId),
            TeseInvestimento,
            TicketMinimo,
            TicketMaximo
        );
    }
}
