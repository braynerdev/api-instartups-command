using Instartups.Command.Domain.Entities.Base;


namespace Instartups.Command.Application.Interfaces.Repositories;

public interface IRepositoriesGeneric<T> where T : BaseEntity
{
    public T Add(T entity);
    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<bool> ExistsAsync(CancellationToken cancellationToken);
    public T Update(T entity);
    public void Remove(T entity);
}
