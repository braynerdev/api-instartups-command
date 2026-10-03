using Microsoft.OpenApi;

namespace Instartups.Command.Api.Configurations;

public static class SwaggerConfig
{
    private static readonly string _apiName = "Instartups Command";
    private static readonly string _apiDescription = "API para o lado de escrita (Command) da Instartups, plataforma que conecta startups do Porto Digital a mentores e investidores anjo do Nordeste. Consultas ficam a cargo da API de Query.";

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = _apiName,
                Version = "v1",
                Description = _apiDescription
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Informe o token JWT no formato: Bearer {seu token}",
            });

            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });

        return services;
    }
}
