using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Annotations;

public interface IGetLabelsByGroupHandler : IRequestHandler<Guid?, IEnumerable<LabelDto>>;

public class GetLabelsByGroupHandler(
    ILabelService labelService,
    ILogger<GetLabelsByGroupHandler> logger
) : IGetLabelsByGroupHandler
{
    public async Task<Result<IEnumerable<LabelDto>>> Handle(
        Guid? groupId,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Getting labels for group: {GroupId}", groupId);

        var labels = await labelService.GetByGroupAsync(groupId, cancellationToken);
        return Result<IEnumerable<LabelDto>>.Success(labels);
    }
}
