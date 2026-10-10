using Instartups.Command.Application.Common.Response;
using Instartups.Command.Domain.Entities;

namespace Instartups.Command.Application.UseCases.EditarPerfilInvestidor;

public sealed record EditarPerfilInvestidorResponse(
    PerfilResponse Perfil,
    string TeseInvestimento,
    decimal TicketMinimo,
    decimal TicketMaximo
)
{
    public static EditarPerfilInvestidorResponse FromEntity(PerfilEntity perfil)
    {
        return new EditarPerfilInvestidorResponse(
            PerfilResponse.FromEntity(perfil),
            perfil.Investidor!.TeseInvestimento,
            perfil.Investidor.TicketMinimo,
            perfil.Investidor.TicketMaximo
        );
    }
}
