using Instartups.Command.Api.Configurations;
using Instartups.Command.Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddSerilogConfig();
builder.AddWolverineConfig();
builder.Services.AddInfrastructureDependencyInjection(builder.Configuration);
builder.Services
    .AddControllersConfig()
    .AddLowerCaseConfig()
    .AddAuthenticationConfig(builder.Configuration)
    .AddAuthorizationConfig()
    .AddSwaggerConfig();



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSerilogConfig();
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerConfig();
}

app.MapControllers();

app.Run();
