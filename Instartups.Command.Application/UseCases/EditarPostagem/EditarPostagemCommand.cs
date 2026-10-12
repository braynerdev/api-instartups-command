using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Interfaces;

namespace Instartups.Command.Application.UseCases.EditarPostagem;

public sealed record EditarPostagemCommand(
    Guid PostagemId,
    string Descricao,
    IEnumerable<Guid> MidiasRemovidas,
    IEnumerable<MidiaCommand> MidiasAdicionadas,
    Guid UserId
) : ICommand;
