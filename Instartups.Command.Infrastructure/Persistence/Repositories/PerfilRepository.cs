using Instartups.Command.Application.Interfaces.Repositories;
using Instartups.Command.Domain.Entities;


namespace Instartups.Command.Infrastructure.Persistence.Repositories;

public class PerfilRepository(
        AppDbContext context
    ) : RepositoriesGeneric<PerfilEntity>(context), IPerfilRepository
{
}
