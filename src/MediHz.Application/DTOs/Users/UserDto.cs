namespace MediHz.Application.DTOs.Users;

public sealed record UserDto(
    Guid UserId,
    string Email,
    string DisplayName,
    DateTimeOffset CreatedAt);
