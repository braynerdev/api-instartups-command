using Instartups.Command.Application.Interfaces;
using Instartups.Command.Application.Interfaces.Repositories;
using Instartups.Command.Infrastructure.Persistence.Repositories;

namespace Instartups.Command.Infrastructure.Persistence;

public class UnitOfWork(
    AppDbContext appDbContext
) : IUnitOfWork
{

    private IPerfilRepository? _perfilRepository;

    public IPerfilRepository Perfil =>
        _perfilRepository ??= new PerfilRepository(appDbContext);

    public async Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        return await appDbContext.SaveChangesAsync(cancellationToken);
    }
}
