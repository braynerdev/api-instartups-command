using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.EntityFrameworkCore;

namespace Instartups.Command.Infrastructure.Persistence;

public static class DatabaseConfig
{
    public static IServiceCollection AddPostgresConf(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var ConnectionString = configuration.GetConnectionString("PostgresConnection")
                               ?? throw new InvalidOperationException("Connection string 'PostgresConnection' não configurada.");

        services.AddDbContextWithWolverineIntegration<AppDbContext>(options =>
            options.UseNpgsql(
                ConnectionString,
                bd =>
                {
                    bd.MigrationsAssembly("Instartups.Command.Infrastructure");
                    bd.CommandTimeout(30);
                    bd.UseNetTopologySuite(); 
                })
        );

        return services;
    }
}