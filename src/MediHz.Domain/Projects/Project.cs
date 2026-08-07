using MediHz.Domain.Audio;
using MediHz.Domain.Core;
using MediHz.Domain.Users;


namespace MediHz.Domain.Projects;

public class Project : EntityBase
{
    public String Name { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    public ICollection<AudioFile> AudioFiles { get; set; } = new List<AudioFile>();
}

public enum ProjectMemberRole
{
    Owner = 0,
    Annotator = 1,
    Viewer = 2
}