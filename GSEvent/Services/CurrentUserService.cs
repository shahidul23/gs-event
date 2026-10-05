using System;
using System.Security.Claims;
using GSEvent.Services.Interfaces;

namespace GSEvent.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContext)
    {
        _httpContextAccessor = httpContext;
    }
    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? UserName => User?.FindFirstValue(ClaimTypes.Name);

    public string? Email => User?.FindFirstValue(ClaimTypes.Email);

    public string? Phone => User?.FindFirstValue(ClaimTypes.MobilePhone);

    public string? FullName => User?.FindFirstValue("fullName");

    public string? Role => User?.FindFirstValue(ClaimTypes.Role);
}
