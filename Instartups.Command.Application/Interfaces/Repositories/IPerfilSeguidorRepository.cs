using Instartups.Command.Domain.Entities;

namespace Instartups.Command.Application.Interfaces.Repositories;

public interface IPerfilSeguidorRepository : IRepositoriesGeneric<PerfilSeguidorEntity>
{
    public Task<bool> JaSegueAsync(Guid seguidorId, Guid seguidoId, CancellationToken ct);
    public Task<PerfilSeguidorEntity?> ObterComSeguidoAsync(Guid seguidorId, Guid seguidoId, CancellationToken ct);
}
