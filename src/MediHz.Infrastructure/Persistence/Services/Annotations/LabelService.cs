using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;
using MediHz.Domain.Annotations;
using Microsoft.EntityFrameworkCore;

namespace MediHz.Infrastructure.Persistence.Services.Annotations;

public class LabelService(MediHzDbContext dbContext) : ILabelService
{
    public async Task<IReadOnlyList<LabelDto>> GetByGroupAsync(
        Guid? labelGroupId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Labels.Where(l => l.LabelGroupId == labelGroupId)
            .Select(l => new LabelDto(l.Id, l.Name, l.LabelGroupId, l.OwnerId, l.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LabelDto>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Labels.Select(l => new LabelDto(
                l.Id,
                l.Name,
                l.LabelGroupId,
                l.OwnerId,
                l.CreatedAtUtc
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<LabelDto> CreateAsync(
        Guid ownerId,
        CreateLabelRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (request.LabelGroupId.HasValue)
        {
            var groupExists = await dbContext.LabelGroups.AnyAsync(
                lg => lg.Id == request.LabelGroupId.Value,
                cancellationToken
            );

            if (!groupExists)
                throw new KeyNotFoundException("The specified label group does not exist.");
        }

        var label = new Label
        {
            Name = request.Name,
            OwnerId = ownerId,
            LabelGroupId = request.LabelGroupId,
        };
        await dbContext.Labels.AddAsync(label, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new LabelDto(
            label.Id,
            label.Name,
            label.LabelGroupId,
            label.OwnerId,
            label.CreatedAtUtc
        );
    }

    public async Task<bool> DeleteAsync(Guid labelId, CancellationToken cancellationToken = default)
    {
        var label = await dbContext.Labels.FindAsync([labelId], cancellationToken);
        if (label == null)
            return false;

        if (label.OwnerId == null)
            throw new InvalidOperationException("Cannot delete pre-packaged labels.");

        dbContext.Labels.Remove(label);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<LabelDto?> AssignToGroupAsync(
        Guid labelId,
        Guid? targetGroupId,
        CancellationToken cancellationToken = default
    )
    {
        var label = await dbContext.Labels.FindAsync([labelId], cancellationToken);
        if (label == null)
            return null;

        if (
            targetGroupId.HasValue
            && !await dbContext.LabelGroups.AnyAsync(
                lg => lg.Id == targetGroupId.Value,
                cancellationToken
            )
        )
            return null;

        label.LabelGroupId = targetGroupId;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new LabelDto(
            label.Id,
            label.Name,
            label.LabelGroupId,
            label.OwnerId,
            label.CreatedAtUtc
        );
    }
}
