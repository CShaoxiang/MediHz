using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediHz.Infrastructure.Persistence;
using MediHz.Infrastructure.Services;

namespace MediHz.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MediHz")
            ?? throw new InvalidOperationException("Connection string 'MediHz' is missing.");

        services.AddDbContext<MediHzDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddScoped<IAudioFileUploadService,  AudioFileUploadService>();
        
        services.AddScoped<IStorageService, LocalStorageService>();
        return services;
    }
}
