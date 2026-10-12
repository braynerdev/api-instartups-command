using Instartups.Command.Application.Interfaces.Repositories;
using Instartups.Command.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Infrastructure.Persistence.Repositories;

public abstract class RepositoriesGeneric<T>(
        AppDbContext context
    ) : IRepositoriesGeneric<T>
    where T : BaseEntity
{
    public T Add(T entity)
    {
        context.Set<T>().Add(entity);
        return entity;
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Set<T>().AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(CancellationToken cancellationToken)
    {
        return await context.Set<T>().AnyAsync(cancellationToken);
    }

    public T Update(T entity)
    {
        context.Update(entity);
        return entity;
    }

    public void Remove(T entity)
    {
        context.Set<T>().Remove(entity);
    }
}
