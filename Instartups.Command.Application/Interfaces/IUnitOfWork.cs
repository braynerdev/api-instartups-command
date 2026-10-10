using Instartups.Command.Application.Interfaces.Repositories;

namespace Instartups.Command.Application.Interfaces;

public interface IUnitOfWork
{
    IPerfilRepository Perfil { get; }
    IPerfilSeguidorRepository PerfilSeguidor { get; }

    public Task<int> CommitAsync(CancellationToken cancellationToken);
}
