using MediHz.Domain.Core;
using MediHz.Domain.Users;

namespace MediHz.Domain.Annotations;

public class Label : EntityBase
{
    public Guid? LabelGroupId { get; set; }
    public LabelGroup? LabelGroup { get; set; }

    public Guid? OwnerId { get; set; }
    public User? Owner { get; set; } = null;

    public string Name { get; set; } = string.Empty;
}
