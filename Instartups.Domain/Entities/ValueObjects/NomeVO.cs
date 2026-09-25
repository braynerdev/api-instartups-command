namespace Instartups.Domain.Entities.ValueObjects;

public sealed record NomeVO
{
    public string Nome { get; }

    private NomeVO(string nome)
    {
        Nome = nome.ToUpperInvariant();
    }

    public static NomeVO Create(string nome)
    {
        return new NomeVO(nome);
    }
}
