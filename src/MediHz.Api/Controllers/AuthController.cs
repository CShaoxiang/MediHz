using MediHz.Api.Controllers;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger)
    : ControllerBase
{
    [HttpPost("google")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Google(
        GoogleLoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await authService.GoogleLoginAsync(request, cancellationToken);
        return Ok(response);
    }
}
