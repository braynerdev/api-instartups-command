using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Domain.Services;

public static class DesativarPostagemService
{
    public static void Desativar(PostagemEntity postagem, PerfilEntity autor)
    {
        if (!autor.Ativo)
            throw new PerfilDesativadoException();

        if (postagem.AutorId != autor.Id)
            throw new PerfilNaoEhAutorPostagemException();

        postagem.Desativar();
        autor.RemoverCurtidas(postagem.TotalCurtidas);
    }
}
