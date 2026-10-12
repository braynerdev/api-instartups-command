using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.UseCases.CadastrarPostagem;
using Instartups.Command.Application.UseCases.EditarPostagem;
using Instartups.Command.Domain.Enums;

namespace Instartups.Command.Tests.Common.Builders.Commands;

public static class PostagemCommandBuilder
{
    public static MidiaCommand Midia(int indice = 0, TiposMidiaEnum tipo = TiposMidiaEnum.FOTO) =>
        new($"https://cdn.instartups.com/nova-midia-{indice}.png", tipo);

    public static IReadOnlyList<MidiaCommand> Midias(int quantidade) =>
        Enumerable.Range(0, quantidade).Select(i => Midia(i)).ToList();

    public static CadastrarPostagemCommand CadastrarPostagem(Guid? autorId = null, int quantidadeMidias = 1) => new(
        Descricao: "Acabamos de fechar nossa rodada seed!",
        AutorId: autorId ?? Guid.NewGuid(),
        Midias: Midias(quantidadeMidias)
    );

    public static EditarPostagemCommand EditarPostagem(Guid? postagemId = null, Guid? userId = null) => new(
        PostagemId: postagemId ?? Guid.NewGuid(),
        Descricao: "Descrição editada da postagem.",
        MidiasRemovidas: [],
        MidiasAdicionadas: [],
        UserId: userId ?? Guid.NewGuid()
    );
}
