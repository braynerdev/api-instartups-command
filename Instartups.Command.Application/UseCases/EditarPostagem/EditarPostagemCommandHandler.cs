using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Application.UseCases.EditarPostagem;

public class EditarPostagemCommandHandler(
        IUnitOfWork unitOfWork
    ) : IVoidCommandHandler<EditarPostagemCommand>
{
    public async Task Handle(EditarPostagemCommand command, CancellationToken ct)
    {
        var perfil = await ObterPerfil(command.UserId, ct);
        var postagem = await ObterPostagem(command.PostagemId, ct);

        ValidarAutor(postagem, perfil);

        postagem.EditarDescricao(command.Descricao);
        RemoverMidias(postagem, command.MidiasRemovidas);
        AdicionarMidias(postagem, command.MidiasAdicionadas);

        await unitOfWork.CommitAsync(ct);
    }

    private async Task<PerfilEntity> ObterPerfil(Guid usuarioId, CancellationToken ct)
    {
        var perfil = await unitOfWork.Perfil.ObterPorUsuarioIdAsync(usuarioId, ct)
            ?? throw new PerfilNaoEncontradoException();

        if (!perfil.Ativo)
            throw new PerfilDesativadoException();

        return perfil;
    }

    private async Task<PostagemEntity> ObterPostagem(Guid postagemId, CancellationToken ct)
    {
        var postagem = await unitOfWork.Postagem.ObterPorIdComMidiasAsync(postagemId, ct);

        return postagem ?? throw new PostagemNaoEncontradaException();
    }

    private static void ValidarAutor(PostagemEntity postagem, PerfilEntity perfil)
    {
        if (postagem.AutorId != perfil.Id)
            throw new PerfilNaoEhAutorPostagemException();
    }
    
    private static void RemoverMidias(PostagemEntity postagem, IEnumerable<Guid> midiasRemovidas)
    {
        foreach (var midiaId in midiasRemovidas)
        {
            postagem.RemoverMidiaPostagem(midiaId);
        }
    }

    private static void AdicionarMidias(PostagemEntity postagem, IEnumerable<MidiaCommand> midiasAdicionadas)
    {
        foreach (var midia in midiasAdicionadas)
        {
            postagem.AdicionarMidiaPostagem(midia.Url, midia.Tipo);
        }
    }
}
