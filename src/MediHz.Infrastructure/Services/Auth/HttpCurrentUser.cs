using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace MediHz.Infrastructure.Services;

using System.Security.Claims;
using MediHz.Application.Abstractions.Auth;

public class HttpCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal claimsPrincipal =>
        httpContextAccessor.HttpContext?.User ?? throw new UnauthorizedAccessException();

    public bool IsAuthenticated => claimsPrincipal.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            var subject =
                claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(subject, out var userId))
            {
                throw new UnauthorizedAccessException(
                    "Authenticated user does not have a valid subject claim"
                );
            }

            return userId;
        }
    }
}
