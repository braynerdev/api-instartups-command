using Instartups.Domain.Entities.Base;

namespace Instartups.Domain.Entities;

public class InvestidorEntity : BaseEntity
{
    public string TeseInvestimento { get; private set; } = null!;
    public decimal TicketMinimo { get; private set; }
    public decimal TicketMaximo { get; private set; }
    public Guid PerfilId { get; private set; }
    public PerfilEntity Perfil { get; private set; } = null!;

    private InvestidorEntity(string teseInvestimento, decimal ticketMinimo, decimal ticketMaximo, Guid perfilId)
    {
        TeseInvestimento = teseInvestimento;
        TicketMinimo = ticketMinimo;
        TicketMaximo = ticketMaximo;
        PerfilId = perfilId;
    }

    internal static InvestidorEntity Criar(string teseInvestimento, decimal ticketMinimo, decimal ticketMaximo, Guid perfilId)
    {
        return new InvestidorEntity(teseInvestimento, ticketMinimo, ticketMaximo, perfilId);
    }

    internal InvestidorEntity Editar(string teseInvestimento, decimal ticketMinimo, decimal ticketMaximo)
    {
        TeseInvestimento = teseInvestimento;
        TicketMinimo = ticketMinimo;
        TicketMaximo = ticketMaximo;
        return this;
    }
}
