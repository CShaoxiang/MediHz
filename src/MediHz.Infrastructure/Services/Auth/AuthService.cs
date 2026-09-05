using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Users;
using MediHz.Application.DTOs.Auth;

namespace MediHz.Infrastructure.Services;

public sealed class AuthService(
    IGoogleTokenVerifier _googleTokenVerifier,
    IUserService _userService,
    ITokenService _tokenService
) : IAuthService
{
    public async Task<AuthResponse> GoogleLoginAsync(
        GoogleLoginRequest request,
        CancellationToken cancellationToken = default
    )
    {
        // Verify GoogleID token
        var identity = await _googleTokenVerifier.VerifyIdTokenAsync(
            request.IdToken,
            cancellationToken
        );

        if (!identity.IsEmailVerified)
            throw new UnauthorizedAccessException("Google account is not verified");

        // Upsert user
        var user = await _userService.UpsertFromExternalUserAsync(
            identity.Subject,
            identity.Email,
            identity.DisplayName,
            "Google",
            cancellationToken
        );
        // Issue a JWT
        return _tokenService.CreateAuthResponse(user);
    }
}
