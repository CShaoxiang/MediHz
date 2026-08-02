using MediHz.Api.Hubs;

namespace MediHz.Api.Extensions;

public static class HubRegistration
{
    private const string Prefix = "/hubs";

    public static IEndpointRouteBuilder MapHubs(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<SampleHub>($"{Prefix}/sample");
        return endpoints;
    }
}
