using Instartups.Command.Application.Interfaces.Repositories;
using Instartups.Command.Domain.Entities;

namespace Instartups.Command.Infrastructure.Persistence.Repositories;

public class PostagemRepository : RepositoriesGeneric<PostagemEntity>, IPostagemRepository
{
    private readonly AppDbContext _context;

    public PostagemRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }
}