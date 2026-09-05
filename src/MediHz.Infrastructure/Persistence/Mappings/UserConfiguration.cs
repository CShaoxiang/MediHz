using MediHz.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediHz.Infrastructure.Persistence.Mappings;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Email).HasMaxLength(320).IsRequired();

        builder.Property(user => user.DisplayName).HasMaxLength(150).IsRequired();

        builder.Property(user => user.CreatedAtUtc).IsRequired();

        builder.Property(user => user.ProviderToken).HasMaxLength(255).IsRequired();

        builder.HasIndex(user => user.Email).IsUnique();

        builder.HasIndex(user => user.ProviderToken).IsUnique();
    }
}
