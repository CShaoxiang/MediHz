using MediHz.Application.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediHz.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAudioFileUploadHandler, AudioFileUploadHandler>();

        return services;
    }
}
