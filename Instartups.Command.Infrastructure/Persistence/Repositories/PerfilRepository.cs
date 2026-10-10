using Instartups.Command.Application.Interfaces.Repositories;
using Instartups.Command.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Instartups.Command.Infrastructure.Persistence.Repositories;

public class PerfilRepository(
        AppDbContext context
    ) : RepositoriesGeneric<PerfilEntity>(context), IPerfilRepository
{
    public async Task<bool> UsuarioJaPossuiPerfilAsync(Guid usuarioId, CancellationToken ct)
    {
        return await context.Perfil.AnyAsync(p => p.UsuarioId == usuarioId, ct);
    }

    public async Task<PerfilEntity?> ObterPorUsuarioIdComInvestidorAsync(Guid usuarioId, CancellationToken ct)
    {
        return await context.Perfil
            .Include(p => p.Investidor)
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId, ct);
    }
}
