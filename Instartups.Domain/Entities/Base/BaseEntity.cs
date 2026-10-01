namespace Instartups.Domain.Entities.Base;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public DateTimeOffset DataCriacao { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DataAtualizacao { get; private set; }
    public bool Ativo { get; private set; } = true;

    public BaseEntity()
    {

    }

    protected void Atualizar()
    {
        DataAtualizacao = DateTimeOffset.UtcNow;
    }

    public void Ativar()
    {
        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
    }

    public override bool Equals(object? obj) => obj is BaseEntity entity && Id.Equals(entity.Id);
    public override int GetHashCode() => HashCode.Combine(Id);
}
