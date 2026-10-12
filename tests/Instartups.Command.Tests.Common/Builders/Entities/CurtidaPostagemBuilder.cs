using Instartups.Command.Domain.Entities;
using Instartups.Command.Tests.Common.Extensions;

namespace Instartups.Command.Tests.Common.Builders.Entities;

public sealed class CurtidaPostagemBuilder
{
    private PostagemEntity _postagem = PostagemBuilder.Nova().Build();
    private PerfilEntity _perfilAutorCurtida = PerfilBuilder.Investidor().Build();

    public static CurtidaPostagemBuilder Nova() => new();

    public CurtidaPostagemBuilder DaPostagem(PostagemEntity postagem)
    {
        _postagem = postagem;
        return this;
    }

    public CurtidaPostagemBuilder DoPerfil(PerfilEntity perfil)
    {
        _perfilAutorCurtida = perfil;
        return this;
    }

    public CurtidaPostagemEntity Build()
    {
        return CurtidaPostagemEntity.Criar(_postagem.Id, _perfilAutorCurtida.Id)
            .DefinirPropriedadePrivada(nameof(CurtidaPostagemEntity.Postagem), _postagem)
            .DefinirPropriedadePrivada(nameof(CurtidaPostagemEntity.PerfilAutorCurtida), _perfilAutorCurtida);
    }
}
