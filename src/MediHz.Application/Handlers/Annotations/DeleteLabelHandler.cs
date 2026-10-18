using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Annotations;

public interface IDeleteLabelHandler : IRequestHandler<Guid, LabelDto?>;

public class DeleteLabelHandler(ILabelService labelService, ILogger<DeleteLabelHandler> logger)
    : IDeleteLabelHandler
{
    public async Task<Result<LabelDto?>> Handle(Guid labelId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting label: {LabelId}", labelId);

        try
        {
            var deleted = await labelService.DeleteAsync(labelId, cancellationToken);
            return deleted
                ? Result<LabelDto?>.Success(null, 204)
                : Result<LabelDto?>.Failure("Not Found", "Label not found.", 404);
        }
        catch (InvalidOperationException ex)
        {
            return Result<LabelDto?>.Failure(
                "Forbidden",
                ex.Message,
                StatusCodes.Status403Forbidden
            );
        }
    }
}
