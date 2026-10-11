using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Domain.Services;

public static class CurtirPostagemService
{
    public static CurtidaPostagemEntity Curtir(PostagemEntity postagem, PerfilEntity autorPostagem, PerfilEntity perfilQueCurtiu)
    {
        ValidarAutor(postagem, autorPostagem);

        if (!perfilQueCurtiu.Ativo)
            throw new PerfilDesativadoException();

        var curtida = CurtidaPostagemEntity.Criar(postagem.Id, perfilQueCurtiu.Id);
        postagem.AdicionarCurtida();
        autorPostagem.AdicionarCurtida();
        return curtida;
    }

    public static void Descurtir(PostagemEntity postagem, PerfilEntity autorPostagem)
    {
        ValidarAutor(postagem, autorPostagem);

        postagem.RemoverCurtida();
        autorPostagem.RemoverCurtida();
    }

    private static void ValidarAutor(PostagemEntity postagem, PerfilEntity autorPostagem)
    {
        if (postagem.AutorId != autorPostagem.Id)
            throw new ArgumentException("O perfil informado não é o autor da postagem.", nameof(autorPostagem));
    }
}
