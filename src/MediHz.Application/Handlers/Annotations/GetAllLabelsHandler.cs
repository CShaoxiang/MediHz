using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;

namespace MediHz.Application.Handlers.Annotations;

public interface IGetAllLabelsHandler : IRequestHandler<EmptyRequest, IEnumerable<LabelDto>>;

public class GetAllLabelsHandler(ILabelService labelService) : IGetAllLabelsHandler
{
    public async Task<Result<IEnumerable<LabelDto>>> Handle(
        EmptyRequest _,
        CancellationToken cancellationToken
    )
    {
        var labels = await labelService.GetAllAsync(cancellationToken);
        return Result<IEnumerable<LabelDto>>.Success(labels);
    }
}
