using Instartups.Command.Application.Interfaces.Repositories;
using Instartups.Command.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Instartups.Command.Infrastructure.Persistence.Repositories;

public class CurtidaPostagemRepository : RepositoriesGeneric<CurtidaPostagemEntity>, ICurtidaPostagemRepository
{
    private readonly AppDbContext _context;

    public CurtidaPostagemRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<bool> JaCurtiuAsync(Guid postagemId, Guid perfilId, CancellationToken ct)
    {
        return await _context.CurtidaPostagem
            .AnyAsync(cp => cp.PostagemId == postagemId && cp.PerfilAutorCurtidaId == perfilId, ct);
    }

    public async Task<CurtidaPostagemEntity?> ObterComPostagemEAutorAsync(Guid postagemId, Guid perfilId, CancellationToken ct)
    {
        return await _context.CurtidaPostagem
            .Include(cp => cp.Postagem)
                .ThenInclude(p => p.Autor)
            .FirstOrDefaultAsync(cp => cp.PostagemId == postagemId && cp.PerfilAutorCurtidaId == perfilId, ct);
    }
}
