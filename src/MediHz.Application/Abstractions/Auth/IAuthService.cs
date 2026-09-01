using MediHz.Application.DTOs.Auth;

namespace MediHz.Application.Abstractions.Auth;

public interface IAuthService
{
    // Complete login use case 
    Task<AuthResponse> GoogleLoginAsync(
        GoogleLoginRequest request,
        CancellationToken cancellationToken = default);
}