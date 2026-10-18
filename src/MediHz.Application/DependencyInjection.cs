using MediHz.Application.Handlers.Annotations;
using MediHz.Application.Handlers.Audio;
using MediHz.Application.Handlers.Exports;
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
        services.AddScoped<IAudioFileFetchHandler, AudioFileFetchHandler>();
        services.AddScoped<IAudioFilePatchHandler, AudioFilePatchHandler>();
        services.AddScoped<IAudioFileDeleteHandler, AudioFileDeleteHandler>();

        // Projects
        services.AddScoped<IAddProjectMemberHandler, AddProjectMemberHandler>();
        services.AddScoped<
            MediHz.Application.Abstractions.Projects.IProjectAccessService,
            MediHz.Application.Services.ProjectAccessService
        >();
        services.AddScoped<ICreateProjectHandler, CreateProjectHandler>();
        services.AddScoped<IGetProjectsHandler, GetProjectsHandler>();
        services.AddScoped<IGetProjectAudioFilesHandler, GetProjectAudioFilesHandler>();
        services.AddScoped<IGetProjectSingleAudioHandler, GetProjectSingleAudioHandler>();
        services.AddScoped<IPatchProjectHandler, PatchProjectHandler>();
        services.AddScoped<IDeleteProjectHandler, DeleteProjectHandler>();

        // Annotations
        services.AddScoped<IGetAnnotationsByFileHandler, GetAnnotationsByFileHandler>();
        services.AddScoped<ISaveAnnotationHandler, SaveAnnotationHandler>();
        services.AddScoped<IUpdateAnnotationHandler, UpdateAnnotationHandler>();
        services.AddScoped<IDeleteAnnotationHandler, DeleteAnnotationHandler>();
        services.AddScoped<ICreateLabelGroupHandler, CreateLabelGroupHandler>();
        services.AddScoped<IDeleteLabelGroupHandler, DeleteLabelGroupHandler>();
        services.AddScoped<IGetAllLabelGroupsHandler, GetAllLabelGroupsHandler>();
        services.AddScoped<IGetAllLabelsHandler, GetAllLabelsHandler>();
        services.AddScoped<ICreateLabelHandler, CreateLabelHandler>();
        services.AddScoped<IGetLabelsByGroupHandler, GetLabelsByGroupHandler>();
        services.AddScoped<IDeleteLabelHandler, DeleteLabelHandler>();
        services.AddScoped<IAssignLabelToGroupHandler, AssignLabelToGroupHandler>();

        // Exports
        services.AddScoped<IExportProjectHandler, ExportProjectHandler>();
        services.AddScoped<IExportFileHandler, ExportFileHandler>();

        return services;
    }
}
