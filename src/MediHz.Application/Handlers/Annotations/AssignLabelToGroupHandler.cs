using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Annotations;

public interface IAssignLabelToGroupHandler : IRequestHandler<AssignLabelRequest, LabelDto>;

public class AssignLabelToGroupHandler(
    ILabelService labelService,
    ILogger<AssignLabelToGroupHandler> logger
) : IAssignLabelToGroupHandler
{
    public async Task<Result<LabelDto>> Handle(
        AssignLabelRequest request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Assigning label {LabelId} to group {GroupId}",
            request.LabelId,
            request.TargetGroupId
        );

        var label = await labelService.AssignToGroupAsync(
            request.LabelId,
            request.TargetGroupId,
            cancellationToken
        );
        if (label == null)
            return Result<LabelDto>.Failure(
                "Not Found",
                "Label or target group not found.",
                StatusCodes.Status404NotFound
            );

        return Result<LabelDto>.Success(label);
    }
}
