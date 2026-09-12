using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;
using MediHz.Domain.Annotations;
using Microsoft.EntityFrameworkCore;

namespace MediHz.Infrastructure.Persistence.Services.Annotations;

public class LabelGroupService(MediHzDbContext dbContext) : ILabelGroupService
{
    public async Task<IReadOnlyList<LabelGroupDto>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .LabelGroups.Select(lg => new LabelGroupDto(
                lg.Id,
                lg.Name,
                lg.OwnerId,
                lg.CreatedAtUtc
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<LabelGroupDto> CreateAsync(
        Guid ownerId,
        CreateLabelGroupRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var ownerExists = await dbContext.Users.AnyAsync(
            user => user.Id == ownerId,
            cancellationToken
        );

        if (!ownerExists)
            throw new UnauthorizedAccessException("The authenticated user does not exist.");

        var labelGroup = new LabelGroup { Name = request.Name, OwnerId = ownerId };
        await dbContext.LabelGroups.AddAsync(labelGroup, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new LabelGroupDto(
            labelGroup.Id,
            labelGroup.Name,
            labelGroup.OwnerId,
            labelGroup.CreatedAtUtc
        );
    }

    public async Task<bool> DeleteAsync(
        Guid labelGroupId,
        CancellationToken cancellationToken = default
    )
    {
        var labelGroup = await dbContext.LabelGroups.FindAsync([labelGroupId], cancellationToken);
        if (labelGroup == null)
            return false;

        dbContext.LabelGroups.Remove(labelGroup);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
