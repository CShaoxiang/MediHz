using MediHz.Application.DTOs.Users;

namespace MediHz.Application.Abstractions.Users;

public interface IUserService
{
    /// <summary>
    /// TODO: Return the user profile for the given identifier.
    /// </summary>
    Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// TODO: Look up a user by email for authentication onboarding and ownership checks.
    /// </summary>
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// TODO: Create a user record from the authentication flow if one does not already exist.
    /// </summary>
    Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// TODO: Update the user's display name.
    /// </summary>
    Task<UserDto?> UpdateAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// TODO: Remove a user when the account can safely be deleted.
    /// </summary>
    Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
}
