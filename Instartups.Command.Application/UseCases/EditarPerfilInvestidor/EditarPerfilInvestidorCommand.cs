using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.EditarPerfilInvestidor;

public sealed record EditarPerfilInvestidorCommand(
    EditarPerfilCommand Perfil,
    string TeseInvestimento,
    decimal TicketMinimo,
    decimal TicketMaximo
) : ICommand;
