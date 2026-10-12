using Instartups.Command.Domain.Entities;

namespace Instartups.Command.Application.Interfaces.Repositories;

public interface ICurtidaPostagemRepository : IRepositoriesGeneric<CurtidaPostagemEntity>
{
    public Task<bool> JaCurtiuAsync(Guid postagemId, Guid perfilId, CancellationToken ct);
    public Task<CurtidaPostagemEntity?> ObterComPostagemEAutorAsync(Guid postagemId, Guid perfilId, CancellationToken ct);
}
