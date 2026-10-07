using Instartups.Command.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Infrastructure.Persistence;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }


    public DbSet<PerfilEntity>? Perfil { get; set; }
    public DbSet<StartupEntity>? Startup { get; set; }
    public DbSet<InvestidorEntity>? Investidor { get; set; }
    public DbSet<PerfilSeguidorEntity>? PerfilSeguidor { get; set; }
    public DbSet<PostagemEntity>? Postagem { get; set; }
    public DbSet<CurtidaPostagemEntity>? CurtidaPostagem { get; set; }
    public DbSet<MidiaPostagemEntity>? MidiaPostagem { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}