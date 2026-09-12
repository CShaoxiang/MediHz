using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.DTOs.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Annotations;

public interface ICreateLabelHandler : IRequestHandler<CreateLabelRequest, LabelDto>;

public class CreateLabelHandler(
    ILabelService labelService,
    ICurrentUser currentUser,
    ILogger<CreateLabelHandler> logger
) : ICreateLabelHandler
{
    public async Task<Result<LabelDto>> Handle(
        CreateLabelRequest request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Creating a new label.");

        var label = await labelService.CreateAsync(currentUser.UserId, request, cancellationToken);
        return Result<LabelDto>.Success(label, StatusCodes.Status201Created);
    }
}
