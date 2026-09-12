using MediHz.Application.DTOs.Annotations;

namespace MediHz.Application.Abstractions.Annotations;

public interface ILabelGroupService
{
    Task<IReadOnlyList<LabelGroupDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<LabelGroupDto> CreateAsync(
        Guid ownerId,
        CreateLabelGroupRequest request,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteAsync(Guid labelGroupId, CancellationToken cancellationToken = default);
}
