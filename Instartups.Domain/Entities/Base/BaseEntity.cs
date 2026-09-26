namespace Instartups.Domain.Entities.Base;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; private set; }
    public bool Active { get; private set; } = true;

    public BaseEntity()
    {

    }

    protected void Atualizar()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Ativar()
    {
        Active = true;
    }

    public void Desativar()
    {
        Active = false;
    }

    public override bool Equals(object? obj) => obj is BaseEntity entity && Id.Equals(entity.Id);
    public override int GetHashCode() => HashCode.Combine(Id);
}
