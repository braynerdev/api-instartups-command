namespace Instartups.Domain.ValueObjects;

public sealed record NomeVO
{
    public string Nome { get; }

    private NomeVO(string nome)
    {
        Nome = nome.Trim();
    }

    public static NomeVO Create(string nome)
    {
        return new NomeVO(nome);
    }
}
