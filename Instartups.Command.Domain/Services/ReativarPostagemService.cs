using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Domain.Services;

public static class ReativarPostagemService
{
    public static void Reativar(PostagemEntity postagem, PerfilEntity autor)
    {
        if (!autor.Ativo)
            throw new PerfilDesativadoException();

        if (postagem.AutorId != autor.Id)
            throw new PerfilNaoEhAutorPostagemException();

        postagem.Ativar();
        autor.AdicionarCurtidas(postagem.TotalCurtidas);
    }
}
