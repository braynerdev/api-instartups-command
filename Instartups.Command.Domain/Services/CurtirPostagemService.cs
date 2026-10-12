using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Domain.Services;

public static class CurtirPostagemService
{
    public static CurtidaPostagemEntity Curtir(PostagemEntity postagem, PerfilEntity autorPostagem, PerfilEntity perfilQueCurtiu, bool jaCurtiu)
    {
        if (jaCurtiu)
            throw new PostagemJaCurtidaException();

        ValidarAutor(postagem, autorPostagem);
        ValidarPostagemAtiva(postagem);
        ValidarPerfis(autorPostagem, perfilQueCurtiu);

        if (autorPostagem.Id.Equals(perfilQueCurtiu.Id))
            throw new PerfilNaoPodeCurtirPropriaPostagemException();

        var curtida = CurtidaPostagemEntity.Criar(postagem.Id, perfilQueCurtiu.Id);
        postagem.AdicionarCurtida();
        autorPostagem.AdicionarCurtida();
        return curtida;
    }

    public static void Descurtir(PostagemEntity postagem, PerfilEntity autorPostagem, PerfilEntity perfilQueCurtiu)
    {
        ValidarAutor(postagem, autorPostagem);
        ValidarPostagemAtiva(postagem);
        ValidarPerfis(autorPostagem, perfilQueCurtiu);

        postagem.RemoverCurtida();
        autorPostagem.RemoverCurtida();
    }

    private static void ValidarAutor(PostagemEntity postagem, PerfilEntity autorPostagem)
    {
        if (postagem.AutorId != autorPostagem.Id)
            throw new ArgumentException("O perfil informado não é o autor da postagem.", nameof(autorPostagem));
    }

    private static void ValidarPostagemAtiva(PostagemEntity postagem)
    {
        if (!postagem.Ativo)
            throw new PostagemDesativadaException();
    }

    private static void ValidarPerfis(PerfilEntity autorPostagem, PerfilEntity perfilQueCurtiu)
    {
        if (!autorPostagem.Ativo || !perfilQueCurtiu.Ativo)
            throw new PerfilDesativadoException();
    }
}
