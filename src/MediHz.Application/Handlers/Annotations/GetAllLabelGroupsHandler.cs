using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;

namespace MediHz.Application.Handlers.Annotations;

public interface IGetAllLabelGroupsHandler
    : IRequestHandler<EmptyRequest, IEnumerable<LabelGroupDto>>;

public class GetAllLabelGroupsHandler(ILabelGroupService labelGroupService)
    : IGetAllLabelGroupsHandler
{
    public async Task<Result<IEnumerable<LabelGroupDto>>> Handle(
        EmptyRequest _,
        CancellationToken cancellationToken
    )
    {
        var labelGroups = await labelGroupService.GetAllAsync(cancellationToken);
        return Result<IEnumerable<LabelGroupDto>>.Success(labelGroups);
    }
}
