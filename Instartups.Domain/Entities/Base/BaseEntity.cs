namespace Instartups.Domain.Entities.Base;

public class BaseEntity
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; private set; }
    public bool Active { get; private set; } = true;

    public BaseEntity()
    {

    }

    protected void Update()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    protected void Activate()
    {
        Active = true;
    }

    protected void Deactivate()
    {
        Active = false;
    }
}
