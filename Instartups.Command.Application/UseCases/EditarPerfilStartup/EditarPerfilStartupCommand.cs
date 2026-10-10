using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.EditarPerfilStartup;

public sealed record EditarPerfilStartupCommand(
    EditarPerfilCommand Perfil,
    string Pitch,
    DateOnly DataFundacao,
    int TamanhoEquipe,
    decimal ValorBuscado
) : ICommand;
