namespace MediHz.Application.DTOs.Annotations;

public sealed record AssignLabelRequest(Guid LabelId, Guid? TargetGroupId);
