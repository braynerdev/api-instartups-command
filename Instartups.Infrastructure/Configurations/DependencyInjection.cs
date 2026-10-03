using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Instartups.Command.Infrastructure.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddScoped<IExemplo, Exemplo>();
        // services.AddScoped(typeof(IExemploGenerico<>),typeof(ExemploGenerico<>));
        return services;
    }
}
