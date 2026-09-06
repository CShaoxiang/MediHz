using MediHz.Domain.Core;
using MediHz.Domain.Users;

namespace MediHz.Domain.Annotations;

public class LabelGroup : EntityBase
{
    public ICollection<Label> Labels { get; set; } = [];

    public Guid OwnerId { get; set; } = Guid.Empty;
    public User Owner { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
}
