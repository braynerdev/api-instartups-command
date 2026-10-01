namespace Instartups.Domain.ValueObjects;

public sealed record TotalVO
{
    public long Total { get; }

    private TotalVO(long total)
    {
        Total = total;
    }
    public static TotalVO Create(long total)
    {
        if (total < 0)
            throw new ArgumentException("O total não pode ser negativo."); // ajustar

        return new TotalVO(total);
    }
    public static TotalVO Zero() => new(0);
    public TotalVO Adicionar() => new(Total + 1);
    public TotalVO Remover() => new(Math.Max(0, Total - 1));
}
