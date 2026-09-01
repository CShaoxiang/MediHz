using MediHz.Application.DTOs.Users;

namespace MediHz.Application.DTOs.Auth;

public sealed record AuthResponse (
        string AccessToken,
        DateTimeOffset ExpiresIn,
        UserDto User);