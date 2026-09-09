using MediHz.Api.Hubs;

namespace MediHz.Api.Extensions;

public static class HubRegistration
{
    private const string Prefix = "/hubs";

    public static IEndpointRouteBuilder MapHubs(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<AudioHub>($"{Prefix}/audio");
        return endpoints;
    }
}
