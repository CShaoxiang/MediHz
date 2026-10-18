using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Annotations;

public interface IDeleteLabelGroupHandler : IRequestHandler<Guid, LabelGroupDto?>;

public class DeleteLabelGroupHandler(
    ILabelGroupService labelGroupService,
    ILogger<DeleteLabelGroupHandler> logger
) : IDeleteLabelGroupHandler
{
    public async Task<Result<LabelGroupDto?>> Handle(
        Guid labelGroupId,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Deleting label group: {LabelGroupId}", labelGroupId);

        try
        {
            var deleted = await labelGroupService.DeleteAsync(labelGroupId, cancellationToken);
            return deleted
                ? Result<LabelGroupDto?>.Success(null, 204)
                : Result<LabelGroupDto?>.Failure("Not Found", "Label group not found.", 404);
        }
        catch (InvalidOperationException ex)
        {
            return Result<LabelGroupDto?>.Failure(
                "Forbidden",
                ex.Message,
                StatusCodes.Status403Forbidden
            );
        }
    }
}
