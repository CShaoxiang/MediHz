using MediHz.Domain.Audio;
using MediHz.Domain.Core;
using MediHz.Domain.Users;

namespace MediHz.Domain.Projects;

public class Project(string Name, Guid ownerId) : EntityBase
{
    public string Name { get; set; } = Name;
    public Guid OwnerId { get; set; } = ownerId;
    public User Owner { get; set; } = null!;
    public ICollection<ProjectMember> Members { get; set; } = [];
    public ICollection<AudioFile> AudioFiles { get; set; } = [];
}

[System.Text.Json.Serialization.JsonConverter(
    typeof(System.Text.Json.Serialization.JsonStringEnumConverter<ProjectMemberRole>)
)]
public enum ProjectMemberRole
{
    Owner = 0,
    Annotator = 1,
    Viewer = 2,
}
