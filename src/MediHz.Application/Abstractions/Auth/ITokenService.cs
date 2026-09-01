using MediHz.Application.DTOs.Auth;
using MediHz.Application.DTOs.Users;

namespace MediHz.Application.Abstractions.Auth;

public interface ITokenService
{
    // Issue own API JWT token
    AuthResponse CreateAuthResponse(UserDto user);
}