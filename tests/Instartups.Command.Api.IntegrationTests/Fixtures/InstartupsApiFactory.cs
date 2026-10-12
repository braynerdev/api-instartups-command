using System.Security.Cryptography;
using System.Threading.RateLimiting;
using Instartups.Command.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;

namespace Instartups.Command.Api.IntegrationTests.Fixtures;

public class InstartupsApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string ChaveConnectionString = "InstartupsIntegrationDb";

    public const string Issuer = "instartups-auth-tests";
    public const string Audience = "instartups-command-tests";

    private readonly RSA _chaveRsa = RSA.Create(2048);
    private readonly string _caminhoChavePublica = Path.Combine(Path.GetTempPath(), $"instartups-jwt-public-{Guid.NewGuid()}.pem");
    private Respawner? _respawner;

    public string ConnectionString { get; }
    public JwtTokenFactory Tokens { get; }

    protected virtual bool HabilitarRateLimiter => false;

    public InstartupsApiFactory()
    {
        var configuracao = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        ConnectionString = configuracao.GetConnectionString(ChaveConnectionString)
            ?? throw new InvalidOperationException(
                $"Connection string '{ChaveConnectionString}' não encontrada. Defina 'ConnectionStrings:{ChaveConnectionString}' no appsettings.Development.json ou a variável de ambiente 'ConnectionStrings__{ChaveConnectionString}'.");
        GarantirBancoDeTestes(ConnectionString);

        File.WriteAllText(_caminhoChavePublica, _chaveRsa.ExportSubjectPublicKeyInfoPem());
        Tokens = new JwtTokenFactory(_chaveRsa, Issuer, Audience);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Fora de Development o Wolverine exige código pré-gerado (TypeLoadMode.Static).
        builder.UseEnvironment("Development");

        builder.UseSetting("ConnectionStrings:PostgresConnection", ConnectionString);
        builder.UseSetting("JWT:PublicKeyPath", _caminhoChavePublica);
        builder.UseSetting("JWT:Issuer", Issuer);
        builder.UseSetting("JWT:Audience", Audience);
        builder.UseSetting("Serilog:MinimumLevel:Default", "Warning");
        builder.UseSetting("Serilog:MinimumLevel:Override:Instartups.Command.Api.Configurations", "Warning");
        builder.UseSetting("Serilog:MinimumLevel:Override:Instartups.Command.Infrastructure.Configurations", "Warning");

        builder.ConfigureTestServices(services =>
        {
            if (HabilitarRateLimiter)
                return;

            // O limite real (5 req/20s por usuário, anônimos numa partição só) derrubaria os testes com 429.
            services.PostConfigure<RateLimiterOptions>(options =>
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                    _ => RateLimitPartition.GetNoLimiter("testes-integracao")));
        });
    }

    public async ValueTask InitializeAsync()
    {
        await using (var scope = Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        await using var conexao = new NpgsqlConnection(ConnectionString);
        await conexao.OpenAsync();

        _respawner = await Respawner.CreateAsync(conexao, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = ["__EFMigrationsHistory", "spatial_ref_sys"]
        });
    }

    public async Task ResetarBancoAsync()
    {
        await using var conexao = new NpgsqlConnection(ConnectionString);
        await conexao.OpenAsync();
        await _respawner!.ResetAsync(conexao);
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();

        _chaveRsa.Dispose();

        if (File.Exists(_caminhoChavePublica))
            File.Delete(_caminhoChavePublica);

        GC.SuppressFinalize(this);
    }

    private static void GarantirBancoDeTestes(string connectionString)
    {
        var banco = new NpgsqlConnectionStringBuilder(connectionString).Database;

        if (banco is null || !banco.EndsWith("_tests", StringComparison.Ordinal))
            throw new InvalidOperationException(
                $"O banco '{banco}' não termina com '_tests'. Os testes de integração apagam todos os dados do banco e só rodam num banco dedicado.");
    }
}

public sealed class InstartupsApiComRateLimiterFactory : InstartupsApiFactory
{
    protected override bool HabilitarRateLimiter => true;
}
