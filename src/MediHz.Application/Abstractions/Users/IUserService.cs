using MediHz.Application.DTOs.Users;

namespace MediHz.Application.Abstractions.Users;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<UserDto> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default
    );

    Task<UserDto?> UpdateAsync(
        Guid userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserDto> UpsertFromExternalUserAsync(
        string providerSubject,
        string email,
        string displayName,
        string providerName,
        CancellationToken cancellationToken = default
    );
}
