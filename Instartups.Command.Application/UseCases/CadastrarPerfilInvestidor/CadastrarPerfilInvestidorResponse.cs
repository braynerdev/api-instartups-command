using Instartups.Command.Application.Common.Response;
using Instartups.Command.Domain.Entities;

namespace Instartups.Command.Application.UseCases.CadastrarPerfilInvestidor;

public sealed record CadastrarPerfilInvestidorResponse(
    PerfilResponse Perfil,
    string TeseInvestimento,
    decimal TicketMinimo,
    decimal TicketMaximo
)
{
    public static CadastrarPerfilInvestidorResponse FromEntity(PerfilEntity perfil, string teseInvestimento, decimal ticketMinimo, decimal ticketMaximo)
    {
        return new CadastrarPerfilInvestidorResponse(
            PerfilResponse.FromEntity(perfil),
            teseInvestimento,
            ticketMinimo,
            ticketMaximo
        );
    }
}

