using Instartups.Command.Application.Interfaces.Repositories;

namespace Instartups.Command.Application.Interfaces;

public interface IUnitOfWork
{
    IPerfilRepository Perfil { get; }
    IPerfilSeguidorRepository PerfilSeguidor { get; }
    IPostagemRepository Postagem { get; }
    ICurtidaPostagemRepository CurtidaPostagem { get; }

    public Task<int> CommitAsync(CancellationToken cancellationToken);
}
