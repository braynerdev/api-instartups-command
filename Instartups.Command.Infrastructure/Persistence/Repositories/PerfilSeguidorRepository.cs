using Instartups.Command.Application.Interfaces.Repositories;
using Instartups.Command.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Instartups.Command.Infrastructure.Persistence.Repositories;

public class PerfilSeguidorRepository : RepositoriesGeneric<PerfilSeguidorEntity>, IPerfilSeguidorRepository
{
    private readonly AppDbContext _context;

    public PerfilSeguidorRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<bool> JaSegueAsync(Guid seguidorId, Guid seguidoId, CancellationToken ct)
    {
        return await _context.PerfilSeguidor
            .AnyAsync(ps => ps.SeguidorId == seguidorId && ps.SeguidoId == seguidoId, ct);
    }

    public async Task<PerfilSeguidorEntity?> ObterComSeguidoAsync(Guid seguidorId, Guid seguidoId, CancellationToken ct)
    {
        return await _context.PerfilSeguidor
            .Include(ps => ps.Seguido)
            .FirstOrDefaultAsync(ps => ps.SeguidorId == seguidorId && ps.SeguidoId == seguidoId, ct);
    }
}
