using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.DTOs.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Annotations;

public interface ICreateLabelGroupHandler : IRequestHandler<CreateLabelGroupRequest, LabelGroupDto>;

public class CreateLabelGroupHandler(
    ILabelGroupService labelGroupService,
    ICurrentUser currentUser,
    ILogger<CreateLabelGroupHandler> logger
) : ICreateLabelGroupHandler
{
    public async Task<Result<LabelGroupDto>> Handle(
        CreateLabelGroupRequest request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Creating a new label group.");

        var labelGroup = await labelGroupService.CreateAsync(
            currentUser.UserId,
            request,
            cancellationToken
        );
        return Result<LabelGroupDto>.Success(labelGroup, StatusCodes.Status201Created);
    }
}
