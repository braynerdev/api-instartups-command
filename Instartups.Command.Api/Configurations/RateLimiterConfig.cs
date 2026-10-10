using Instartups.Command.Domain.Exceptions.Base;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace Instartups.Command.Api.Configurations;

public static class RateLimiterConfig
{
    public static IServiceCollection AddRateLimiterConfig(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;


            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var userId = PegarIdUsuario(httpContext);
                

                return RateLimitPartition.GetTokenBucketLimiter(
                    userId,
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 5,
                        TokensPerPeriod = 5,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(20),
                        AutoReplenishment = true,
                        QueueLimit = 2,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    });
            });
        });

        return services;
    }

    public static string PegarIdUsuario(HttpContext httpContext)
    {
        return httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? "anonymous";
    }
}