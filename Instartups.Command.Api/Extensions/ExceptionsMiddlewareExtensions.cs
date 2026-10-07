using Instartups.Command.Api.Middleware;

namespace Instartups.Command.Api.Extensions;

public static class ExceptionsMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionsMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionsMiddleware>();
        return app;
    }
}