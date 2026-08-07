namespace MediHz.Application.DTOs.Users;

public sealed record CreateUserRequest(
    string Email,
    string DisplayName);
