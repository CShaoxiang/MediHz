using MediHz.Domain.Core;
using MediHz.Domain.Users;

namespace MediHz.Domain.Annotations;

public class LabelGroup : EntityBase
{
    public ICollection<Label> Labels { get; set; } = [];

    public Guid? OwnerId { get; set; }
    public User? Owner { get; set; }

    public string Name { get; set; } = string.Empty;
}
