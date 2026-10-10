using System.Security.Claims;
using Instartups.Command.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Instartups.Command.Api.Configurations;

public static class AuthorizationConfig
{
    private const string RoleClaimType = "roles";
    public static IServiceCollection AddAuthorizationConfig(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            foreach (var permissao in PermissoesConst.All())
            {
                AddPermissionPolicy(options, permissao);
            }
        });

        return services;
    }

    private static void AddPermissionPolicy(AuthorizationOptions options, string permissao)
    {
        options.AddPolicy(permissao,
            policy =>
            {
                policy.RequireAuthenticatedUser();

                policy.RequireAssertion(context =>
                    context.User.HasClaim(RoleClaimType, PermissoesConst.Admin) ||
                    context.User.HasClaim(RoleClaimType, permissao));
            });
    }
}