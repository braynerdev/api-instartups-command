using Instartups.Command.Domain.Entities;

namespace Instartups.Command.Application.Interfaces.Repositories;

public interface IPostagemRepository : IRepositoriesGeneric<PostagemEntity>
{
    public Task<PostagemEntity?> ObterPorIdComMidiasAsync(Guid postagemId, CancellationToken ct);
}