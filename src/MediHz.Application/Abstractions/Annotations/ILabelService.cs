using MediHz.Application.DTOs.Annotations;

namespace MediHz.Application.Abstractions.Annotations;

public interface ILabelService
{
    Task<IReadOnlyList<LabelDto>> GetByGroupAsync(
        Guid? labelGroupId,
        CancellationToken cancellationToken = default
    );

    Task<LabelDto> CreateAsync(
        Guid ownerId,
        CreateLabelRequest request,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteAsync(Guid labelId, CancellationToken cancellationToken = default);

    Task<LabelDto?> AssignToGroupAsync(
        Guid labelId,
        Guid? targetGroupId,
        CancellationToken cancellationToken = default
    );
}
