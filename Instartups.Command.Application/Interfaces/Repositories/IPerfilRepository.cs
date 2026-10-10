using Instartups.Command.Domain.Entities;

namespace Instartups.Command.Application.Interfaces.Repositories;

public interface IPerfilRepository : IRepositoriesGeneric<PerfilEntity>
{
    public Task<bool> UsuarioJaPossuiPerfilAsync(Guid usuarioId, CancellationToken ct);
    public Task<PerfilEntity?> ObterPorUsuarioIdComInvestidorAsync(Guid usuarioId, CancellationToken ct);
}
