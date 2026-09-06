using MediHz.Domain.Core;
using MediHz.Domain.Projects;

namespace MediHz.Domain.Users;

public class User : EntityBase
{
    public required string Email { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public required string ProviderToken { get; set; }
    public ICollection<Project> OwnedProjects { get; set; } = [];
    public ICollection<ProjectMember> Members { get; set; } = [];
}
