using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Enums;

namespace Instartups.Command.Application.UseCases.CadastrarPerfilStartup;

public sealed record CadastrarPerfilStartupCommand(
    CadastrarPerfilCommand Perfil,
    string Pitch,
    DateOnly DataFundacao,
    int TamanhoEquipe,
    decimal ValorBuscado,
    SetorStartupEnum Setor
) : ICommand;
