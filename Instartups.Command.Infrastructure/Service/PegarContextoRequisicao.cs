using Instartups.Command.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Instartups.Command.Infrastructure.Service;

public class PegarContextoRequisicao(
    IHttpContextAccessor _httpContextAccessor
) : IPegarContextoRequisicao
{
    public Guid UserId =>
        Guid.Parse(
            _httpContextAccessor.HttpContext!.User
                .FindFirst(ClaimTypes.NameIdentifier)!.Value
        );
    public string? UserName =>
        _httpContextAccessor.HttpContext.User?.FindFirst("unique_name")?.Value;

    public string? Email =>
        _httpContextAccessor.HttpContext.User?.FindFirst("Email")?.Value;

    public string? PhoneNumber =>
        _httpContextAccessor.HttpContext.User?.FindFirst("PhoneNumber")?.Value;
}
