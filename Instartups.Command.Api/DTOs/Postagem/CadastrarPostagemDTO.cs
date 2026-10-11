using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.UseCases.CadastrarPostagem;

namespace Instartups.Command.Api.DTOs.Postagem;

public sealed record CadastrarPostagemDTO(
    string Descricao,
    IEnumerable<MidiaDTO> Midias
)
{
    public CadastrarPostagemCommand ToCommand(Guid userId)
    {
        return new CadastrarPostagemCommand(
                Descricao: Descricao,
                AutorId: userId,
                Midias: ToMidias()
            );
    }

    private IEnumerable<MidiaCommand> ToMidias()
    {
        return Midias.Select(x => x.ToCommand());
    }
}