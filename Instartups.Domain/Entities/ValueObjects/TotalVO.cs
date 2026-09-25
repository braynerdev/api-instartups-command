namespace Instartups.Domain.Entities.ValueObjects;

public sealed record TotalVO(long Total)
{
    public TotalVO Adicionar()
        => new(Total + 1);

    public TotalVO Remover()
        => new(Math.Max(0, Total - 1));
}
