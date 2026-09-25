using Instartups.Domain.Enums;

namespace Instartups.Domain.Entities;

public class MidiaPostagem
{
    public string UrlMidia { get; private set; } = null!;
    public TiposMidiaEnum TipoMidia { get; private set; }
    public Guid PostagemId { get; private set; }
    public PostagemEntity Postagem { get; private set; } = null!;
}
