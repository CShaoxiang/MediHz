using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.DTOs.Auth;
using MediHz.Application.DTOs.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MediHz.Infrastructure.Services;

// Issue a JWT token to user with expire time
public class TokenService(IOptions<JwtOptions> jwtOptions) : ITokenService
{
    private JwtOptions _jwt = jwtOptions.Value;

    public AuthResponse CreateAuthResponse(UserDto user)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwt.Expiration);
        var claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.DisplayName),
        };
        // HMAX -SHA256, issuer, audience, WriteToken -> AuthResponse
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new AuthResponse(accessToken, expiresAt, user);
    }
}
