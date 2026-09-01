using Google.Apis.Auth;
using MediHz.Application.Abstractions.Auth;
using Microsoft.Extensions.Options;

namespace MediHz.Infrastructure.Services;
//Validates Google ID token , returns Google identity claums
public class GoogleTokenVerifier (IOptions<GoogleAuthOptions> googleOptions): IGoogleTokenVerifier
{
    public async Task<IGoogleTokenVerifier.GoogleIdentity> VerifyIdTokenAsync(
        string idToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(idToken) || (string.IsNullOrEmpty(googleOptions.Value.ClientId)))
        {
            throw new ArgumentNullException("Google OAuth ID is missing");
        }
        
        var payload = await GoogleJsonWebSignature.ValidateAsync(
            idToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [googleOptions.Value.ClientId],
            });
        return new IGoogleTokenVerifier.GoogleIdentity(
            payload.Subject,
            payload.Email,
            payload.Name ?? string.Empty,
            payload.EmailVerified);
    }
}

public sealed class GoogleAuthOptions
{
    public const string SectionName = "Authentication:Google";
    public string ClientId { get; set; } = string.Empty;
}