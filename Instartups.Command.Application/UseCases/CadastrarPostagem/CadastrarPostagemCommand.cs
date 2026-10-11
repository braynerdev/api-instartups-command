using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Enums;

namespace Instartups.Command.Application.UseCases.CadastrarPostagem;

public sealed record CadastrarPostagemCommand(
        string Descricao,
        Guid AutorId,
        IEnumerable<MidiaCommand> Midias
    ) : ICommand;
    