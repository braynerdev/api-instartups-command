using Instartups.Command.Application.Interfaces.Repositories;

namespace Instartups.Command.Application.Interfaces;

public interface IUnitOfWork
{
    IPerfilRepository Perfil { get; }

    public Task<int> CommitAsync(CancellationToken cancellationToken);
}
