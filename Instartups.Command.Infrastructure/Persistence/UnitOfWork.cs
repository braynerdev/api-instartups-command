using Instartups.Command.Application.Interfaces;
using Instartups.Command.Application.Interfaces.Repositories;
using Instartups.Command.Infrastructure.Persistence.Repositories;

namespace Instartups.Command.Infrastructure.Persistence;

public class UnitOfWork(
    AppDbContext appDbContext
) : IUnitOfWork
{

    private IPerfilRepository? _perfilRepository;
    private IPerfilSeguidorRepository? _perfilSeguidorRepository;
    private IPostagemRepository? _postagemRepository;
    private ICurtidaPostagemRepository? _curtidaPostagemRepository;



    public IPerfilRepository Perfil =>
        _perfilRepository ??= new PerfilRepository(appDbContext);

    public IPerfilSeguidorRepository PerfilSeguidor =>
        _perfilSeguidorRepository ??= new PerfilSeguidorRepository(appDbContext);
    
    public IPostagemRepository Postagem =>
        _postagemRepository ??= new PostagemRepository(appDbContext);

    public ICurtidaPostagemRepository CurtidaPostagem =>
        _curtidaPostagemRepository ??= new CurtidaPostagemRepository(appDbContext);

    public async Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        return await appDbContext.SaveChangesAsync(cancellationToken);
    }
}
