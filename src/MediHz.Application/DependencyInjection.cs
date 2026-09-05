using MediHz.Application.Handlers.Audio;
using MediHz.Application.Handlers.Projects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediHz.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Audio
        services.AddScoped<IAudioFileUploadHandler, AudioFileUploadHandler>();

        // Projects
        services.AddScoped<ICreateProjectHandler, CreateProjectHandler>();
        services.AddScoped<IGetProjectsHandler, GetProjectsHandler>();
        services.AddScoped<IGetProjectAudioFilesHandler, GetProjectAudioFilesHandler>();

        return services;
    }
}
