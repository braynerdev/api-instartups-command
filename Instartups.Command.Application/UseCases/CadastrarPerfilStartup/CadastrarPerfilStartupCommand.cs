using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.CadastrarPerfilStartup;

public sealed record CadastrarPerfilStartupCommand(
    CadastrarPerfilCommand Perfil,
    string Pitch,
    DateOnly DataFundacao,
    int TamanhoEquipe,
    decimal ValorBuscado
) : ICommand;
