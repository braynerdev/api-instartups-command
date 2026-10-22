using Instartups.Command.Api.Configurations;
using Instartups.Command.Api.Extensions;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Infrastructure.Configurations;
using JasperFx;

var builder = WebApplication.CreateBuilder(args);


builder
    .AddSerilogConfig()
    .AddWolverineConfig();

builder.Services.AddInfrastructureDependencyInjection(builder.Configuration);

builder.Services
    .AddControllersConfig()
    .AddLowerCaseConfig()
    .AddAuthenticationConfig(builder.Configuration)
    .AddAuthorizationConfig()
    .AddCorsConfigurations(builder.Configuration)
    .AddRateLimiterConfig()
    .AddOpenApiConfig();

var app = builder.Build();

var executandoComando = args.Length > 0 && !args[0].StartsWith('-');
if (!executandoComando)
    await app.ApplyMigrationsAsync();
       
app.UseCors(OrigensCorsConst.Frontend);

app.UseSerilogConfig();

app.UseExceptionsMiddleware();

if (app.Environment.IsDevelopment())
    app.UseOpenApiConfig();

else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

return await app.RunJasperFxCommands(args);