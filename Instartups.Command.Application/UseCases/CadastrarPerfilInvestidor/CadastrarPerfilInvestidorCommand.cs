using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.CadastrarPerfilInvestidor;

public sealed record CadastrarPerfilInvestidorCommand(
    CadastrarPerfilCommand Perfil,
    string TeseInvestimento,
    decimal TicketMinimo,
    decimal TicketMaximo
) : ICommand;