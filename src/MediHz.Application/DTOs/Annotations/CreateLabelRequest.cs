namespace MediHz.Application.DTOs.Annotations;

public sealed record CreateLabelRequest(string Name, Guid? LabelGroupId = null);
