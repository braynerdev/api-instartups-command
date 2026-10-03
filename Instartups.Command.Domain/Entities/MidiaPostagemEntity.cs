using Instartups.Command.Domain.Entities.Base;
using Instartups.Command.Domain.Enums;

namespace Instartups.Command.Domain.Entities;

public sealed class MidiaPostagemEntity : BaseEntity
{
    public string UrlMidia { get; private set; } = null!;
    public TiposMidiaEnum TipoMidia { get; private set; }
    public Guid PostagemId { get; private set; }
    public PostagemEntity Postagem { get; private set; } = null!;

    private MidiaPostagemEntity() { }
    private MidiaPostagemEntity(string urlMidia, TiposMidiaEnum tipoMidia, Guid postagemId)
        : base()
    {
        UrlMidia = urlMidia;
        TipoMidia = tipoMidia;
        PostagemId = postagemId;
    }

    public static MidiaPostagemEntity Criar(string urlMidia, TiposMidiaEnum tipoMidia, Guid postagemId)
    {
        return new MidiaPostagemEntity(urlMidia, tipoMidia, postagemId);
    }
}
