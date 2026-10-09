using MediHz.Application.Abstractions.Users;
using MediHz.Application.DTOs.Users;
using MediHz.Domain.Users;
using MediHz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediHz.Infrastructure.Services.Users;

public class UserService(MediHzDbContext db) : IUserService
{
    private static UserDto ToDto(User user) =>
        new(user.Id, user.Email, user.DisplayName, user.ProviderToken, user.CreatedAtUtc);

    public async Task<UserDto> UpsertFromExternalUserAsync(
        string providerSubject,
        string email,
        string displayName,
        string providerName,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(providerSubject))
            throw new ArgumentNullException(
                "Provider subject is required.",
                nameof(providerSubject)
            );

        var normalisedEmail = email.Trim().ToLower();
        var name = string.IsNullOrEmpty(displayName) ? normalisedEmail : displayName.Trim();
        var bySubject = await db.Users.FirstOrDefaultAsync(
            u => u.ProviderToken == providerSubject,
            cancellationToken
        );
        // Existing User check  , update profile and saves changes
        if (bySubject is not null)
        {
            if (bySubject.Email != normalisedEmail)
                bySubject.Email = normalisedEmail;
            if (bySubject.DisplayName != name)
                bySubject.DisplayName = name;

            await db.SaveChangesAsync(cancellationToken);

            return ToDto(bySubject);
        }

        // Find by Email and duplicate check , if not : create user entity and inject
        var byEmail = await db.Users.FirstOrDefaultAsync(
            u => u.Email == normalisedEmail,
            cancellationToken
        );
        if (byEmail is not null)
            throw new InvalidOperationException("Email already exists");

        var user = new User
        {
            Email = normalisedEmail,
            DisplayName = name,
            ProviderToken = providerSubject,
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(user);
    }

    public async Task<UserDto?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;
        var normalisedEmail = email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(
            u => u.Email.ToLower() == normalisedEmail,
            cancellationToken
        );
        return user is null ? null : ToDto(user);
    }

    public Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public Task<UserDto?> UpdateAsync(
        Guid userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
