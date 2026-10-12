using Instartups.Command.Application.Interfaces;
using Instartups.Command.Infrastructure.Persistence;
using Instartups.Command.Infrastructure.Service;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Instartups.Command.Infrastructure.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMapster();
        services.AddPostgresConf(configuration);
        services.AddHttpContextAccessor();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPegarContextoRequisicao, PegarContextoRequisicao>();
        return services;
    }
}
