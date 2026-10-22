using MediHz.Api.Hubs;

namespace MediHz.Api.Extensions;

public static class HubRegistration
{
    private const string Prefix = "/hubs";

    public static IEndpointRouteBuilder MapHubs(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<AudioHub>($"{Prefix}/audio");
        endpoints.MapHub<ProjectPresenceHub>($"{Prefix}/presence", options =>
            options.CloseOnAuthenticationExpiration = true);
        return endpoints;
    }
}
