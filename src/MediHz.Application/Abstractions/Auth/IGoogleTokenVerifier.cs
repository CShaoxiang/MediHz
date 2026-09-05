namespace MediHz.Application.Abstractions.Auth;

public interface IGoogleTokenVerifier
{
    public sealed record GoogleIdentity(
        string Subject,
        string Email,
        string DisplayName,
        bool IsEmailVerified
    );

    Task<GoogleIdentity> VerifyIdTokenAsync(
        string idToken,
        CancellationToken cancellationToken = default
    );
}
