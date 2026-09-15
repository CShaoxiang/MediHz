using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Exports;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.Abstractions.Storage;
using MediHz.Application.Abstractions.Users;
using MediHz.Infrastructure.Persistence;
using MediHz.Infrastructure.Persistence.Services.Annotations;
using MediHz.Infrastructure.Persistence.Services.Projects;
using MediHz.Infrastructure.Services;
using MediHz.Infrastructure.Services.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediHz.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString =
            configuration.GetConnectionString("MediHz")
            ?? throw new InvalidOperationException("Connection string 'MediHz' is missing.");

        services.AddDbContext<MediHzDbContext>(options => options.UseNpgsql(connectionString));
        //Bind GoogleOAuth and Jwt settings
        services.Configure<GoogleAuthOptions>(
            configuration.GetSection(GoogleAuthOptions.SectionName)
        );
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAudioFileUploadService, AudioFileUploadService>();
        services.AddScoped<IStorageService, LocalStorageService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IAudioFileService, AudioFileService>();
        services.AddScoped<ILabelGroupService, LabelGroupService>();
        services.AddScoped<ILabelService, LabelService>();
        services.AddScoped<IAnnotationService, AnnotationService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, HttpCurrentUser>();
        services.AddScoped<IGoogleTokenVerifier, GoogleTokenVerifier>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IExportService, ExportService>();

        return services;
    }
}
