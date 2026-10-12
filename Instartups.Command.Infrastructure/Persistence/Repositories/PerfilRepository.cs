using Instartups.Command.Application.Interfaces.Repositories;
using Instartups.Command.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Instartups.Command.Infrastructure.Persistence.Repositories;

public class PerfilRepository : RepositoriesGeneric<PerfilEntity>, IPerfilRepository
{
    private readonly AppDbContext _context;

    public PerfilRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<bool> UsuarioJaPossuiPerfilAsync(Guid usuarioId, CancellationToken ct)
    {
        return await _context.Perfil.AnyAsync(p => p.UsuarioId == usuarioId, ct);
    }

    public async Task<PerfilEntity?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken ct)
    {
        return await _context.Perfil
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId, ct);
    }

    public async Task<PerfilEntity?> ObterPorUsuarioIdComInvestidorAsync(Guid usuarioId, CancellationToken ct)
    {
        return await _context.Perfil
            .Include(p => p.Investidor)
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId, ct);
    }

    public async Task<PerfilEntity?> ObterPorUsuarioIdComStartupAsync(Guid usuarioId, CancellationToken ct)
    {
        return await _context.Perfil
            .Include(p => p.Startup)
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId, ct);
    }
}
