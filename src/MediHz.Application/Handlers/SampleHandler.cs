namespace MediHz.Application.Handlers;

public record SampleRequest(string Name = "World");

public interface ISampleHandler : IRequestHandler<SampleRequest, string>;

public class SampleHandler : ISampleHandler
{
    public async Task<string> Handle(SampleRequest request)
    {
        return $"Hello, {request.Name}!";
    }
}
