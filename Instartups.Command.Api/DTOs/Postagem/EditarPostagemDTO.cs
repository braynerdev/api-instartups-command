using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.UseCases.EditarPostagem;

namespace Instartups.Command.Api.DTOs.Postagem;

public sealed record EditarPostagemDTO(
    string Descricao,
    IEnumerable<Guid>? MidiasRemovidas,
    IEnumerable<MidiaDTO>? MidiasAdicionadas
)
{
    public EditarPostagemCommand ToCommand(Guid postagemId, Guid userId)
    {
        return new EditarPostagemCommand(
                PostagemId: postagemId,
                Descricao: Descricao,
                MidiasRemovidas: MidiasRemovidas ?? [],
                MidiasAdicionadas: ToMidias(),
                UserId: userId
            );
    }

    private IEnumerable<MidiaCommand> ToMidias()
    {
        return MidiasAdicionadas?.Select(x => x?.ToCommand()!) ?? [];
    }
}
