using Instartups.Command.Application.Interfaces.Repositories;
using Instartups.Command.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Infrastructure.Persistence.Repositories;

public class PostagemRepository : RepositoriesGeneric<PostagemEntity>, IPostagemRepository
{
    private readonly AppDbContext _context;

    public PostagemRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<PostagemEntity?> ObterPorIdComMidiasAsync(Guid postagemId, CancellationToken ct)
    {
        return await _context.Postagem
            .Include(p => p.MidiasPostagem)
            .FirstOrDefaultAsync(p => p.Id == postagemId, ct);
    }

    public async Task<PostagemEntity?> ObterPorIdComAutorAsync(Guid postagemId, CancellationToken ct)
    {
        return await _context.Postagem
            .Include(p => p.Autor)
            .FirstOrDefaultAsync(p => p.Id == postagemId, ct);
    }
}