using System.Text.Json;
using MediHz.Api.Hubs;
using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Annotations;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.DTOs.Projects;
using MediHz.Application.Handlers.Annotations;
using MediHz.Application.Handlers.Audio;
using MediHz.Application.Handlers.Projects;
using MediHz.Application.Services;
using MediHz.Domain.Annotations;
using MediHz.Domain.Audio;
using MediHz.Domain.Projects;
using MediHz.Domain.Users;
using MediHz.Infrastructure.Persistence;
using MediHz.Infrastructure.Persistence.Services.Annotations;
using MediHz.Infrastructure.Persistence.Services.Projects;
using MediHz.Infrastructure.Services;
using MediHz.Infrastructure.Services.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MediHz.Tests;

public class ProjectMembershipTests : IDisposable
{
    private readonly MediHzDbContext db = new(
        new DbContextOptionsBuilder<MediHzDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options
    );
    private readonly User owner = new()
    {
        Email = "alice@example.com",
        DisplayName = "Alice",
        ProviderToken = "alice",
    };
    private readonly User target = new()
    {
        Email = "Bob@Example.com",
        DisplayName = "Bob",
        ProviderToken = "bob",
    };
    private readonly Mock<ICurrentUser> actor = new();
    private readonly Project project;
    private readonly AudioFile audio;
    private readonly Annotation annotation;
    private readonly ProjectService projects;
    private readonly AudioFileService audioFiles;
    private readonly AnnotationService annotations;
    private readonly ProjectAccessService access;
    private readonly AddProjectMemberHandler add;
    private readonly UserService users;
    private static CancellationToken Token => CancellationToken.None;

    public ProjectMembershipTests()
    {
        project = new("Shared project", owner.Id);
        audio = new()
        {
            ProjectId = project.Id,
            FileName = "example.wav",
            Format = AudioFileFormat.Wav,
            Channels = 1,
            SampleRate = 8000,
        };
        annotation = new()
        {
            AudioFileId = audio.Id,
            UserId = owner.Id,
            StartTimeSeconds = 0,
            EndTimeSeconds = 1,
            DurationSeconds = 1,
            UpdatedAtUtc = DateTimeOffset.UtcNow,
        };
        db.AddRange(owner, target, project, audio, annotation);
        db.SaveChanges();
        actor.SetupGet(x => x.UserId).Returns(owner.Id);
        actor.SetupGet(x => x.IsAuthenticated).Returns(true);
        projects = new(db);
        audioFiles = new(db);
        annotations = new(db);
        access = new(projects, audioFiles, annotations, actor.Object);
        users = new(db);
        add = new(projects, access, users, NullLogger<AddProjectMemberHandler>.Instance);
    }

    public void Dispose() => db.Dispose();

    [Theory]
    [InlineData(ProjectMemberRole.Annotator)]
    [InlineData(ProjectMemberRole.Viewer)]
    public async Task EmailAddition_GrantsPersistedVisibilityAndRole(ProjectMemberRole role)
    {
        Assert.Empty(await projects.GetForUserAsync(target.Id));
        Assert.Null(await projects.GetByIdAsync(project.Id, target.Id));
        var result = await add.Handle(new(project.Id, "  BOB@example.COM  ", role), Token);
        Assert.Equal(201, result.StatusCode);
        Assert.Equal(target.Id, result.Value!.UserId);
        Assert.Equal(target.DisplayName, result.Value.DisplayName);
        Assert.Equal(target.Email, result.Value.Email);
        Assert.Equal(role, result.Value.Role);
        Assert.NotEqual(default, result.Value.JoinedAt);
        db.ChangeTracker.Clear();
        var visible = Assert.Single(await projects.GetForUserAsync(target.Id));
        Assert.Equal(project.Id, visible.Id);
        Assert.Equal(role, visible.Role);
        Assert.Equal("Alice", visible.OwnerDisplayName);
        actor.SetupGet(x => x.UserId).Returns(target.Id);
        var details = await new GetProjectsHandler(
            projects,
            actor.Object,
            NullLogger<GetProjectsHandler>.Instance
        ).GetByIdAsync(project.Id, Token);
        Assert.Equal(200, details.StatusCode);
        Assert.Equal(role, details.Value!.Role);
        Assert.Equal(
            ProjectMemberRole.Owner,
            (await projects.GetByIdAsync(project.Id, owner.Id))!.Role
        );
    }

    [Theory]
    [InlineData("", ProjectMemberRole.Annotator)]
    [InlineData("   ", ProjectMemberRole.Viewer)]
    [InlineData(null, ProjectMemberRole.Viewer)]
    [InlineData("not-an-email", ProjectMemberRole.Annotator)]
    [InlineData("bob @example.com", ProjectMemberRole.Annotator)]
    [InlineData("Bob <bob@example.com>", ProjectMemberRole.Annotator)]
    [InlineData("bob@example.com", ProjectMemberRole.Owner)]
    [InlineData("bob@example.com", (ProjectMemberRole)99)]
    public async Task InvalidInput_DoesNotCreateMembership(string? email, ProjectMemberRole role)
    {
        var result = await add.Handle(new(project.Id, email!, role), Token);
        Assert.Equal(400, result.StatusCode);
        Assert.Empty(db.ProjectMembers);
    }

    [Fact]
    public async Task MissingUser_AndOwnerTarget_HaveSpecificErrors()
    {
        Assert.Equal(
            404,
            (
                await add.Handle(
                    new(project.Id, "missing@example.com", ProjectMemberRole.Viewer),
                    Token
                )
            ).StatusCode
        );
        Assert.Equal(
            409,
            (
                await add.Handle(new(project.Id, owner.Email, ProjectMemberRole.Viewer), Token)
            ).StatusCode
        );
        Assert.Empty(db.ProjectMembers);
        Assert.Null(await users.GetByEmailAsync("missing@example.com"));
    }

    [Fact]
    public async Task Duplicate_DoesNotChangeOriginalRoleOrJoinTime()
    {
        var first = await add.Handle(
            new(project.Id, target.Email, ProjectMemberRole.Viewer),
            Token
        );
        var second = await add.Handle(
            new(project.Id, target.Email, ProjectMemberRole.Annotator),
            Token
        );
        Assert.Equal(409, second.StatusCode);
        var row = Assert.Single(db.ProjectMembers);
        Assert.Equal(ProjectMemberRole.Viewer, row.Role);
        Assert.Equal(first.Value!.JoinedAt, row.JoinedAt);
    }

    [Theory]
    [InlineData(ProjectMemberRole.Annotator, 403)]
    [InlineData(ProjectMemberRole.Viewer, 403)]
    [InlineData(null, 404)]
    public async Task OnlyOwnerCanAdd(ProjectMemberRole? role, int status)
    {
        if (role.HasValue)
            await projects.AddMemberAsync(project.Id, target.Id, role.Value);
        actor.SetupGet(x => x.UserId).Returns(target.Id);
        Assert.Equal(
            status,
            (
                await add.Handle(new(project.Id, owner.Email, ProjectMemberRole.Viewer), Token)
            ).StatusCode
        );
        Assert.Equal(
            404,
            (
                await add.Handle(new(Guid.NewGuid(), target.Email, ProjectMemberRole.Viewer), Token)
            ).StatusCode
        );
    }

    [Theory]
    [InlineData(ProjectMemberRole.Owner, true, true, true)]
    [InlineData(ProjectMemberRole.Annotator, true, true, false)]
    [InlineData(ProjectMemberRole.Viewer, true, false, false)]
    [InlineData(null, false, false, false)]
    public async Task Permissions_AreEnforcedForProjectAudioAndAnnotations(
        ProjectMemberRole? role,
        bool read,
        bool write,
        bool manage
    )
    {
        if (role != ProjectMemberRole.Owner)
        {
            if (role.HasValue)
                await projects.AddMemberAsync(project.Id, target.Id, role.Value);
            actor.SetupGet(x => x.UserId).Returns(target.Id);
        }
        Assert.Equal(
            read,
            await access.CheckProjectAsync(project.Id, ProjectPermission.Read, Token) is null
        );
        Assert.Equal(
            write,
            await access.CheckProjectAsync(project.Id, ProjectPermission.Write, Token) is null
        );
        Assert.Equal(
            manage,
            await access.CheckProjectAsync(project.Id, ProjectPermission.Manage, Token) is null
        );
        Assert.Equal(
            read,
            await access.CheckAudioAsync(audio.Id, project.Id, ProjectPermission.Read, Token)
                is null
        );
        Assert.Equal(
            write,
            await access.CheckAnnotationAsync(
                annotation.Id,
                audio.Id,
                ProjectPermission.Write,
                Token
            )
                is null
        );

        var listing = new GetProjectAudioFilesHandler(
            access,
            audioFiles,
            NullLogger<GetProjectAudioFilesHandler>.Instance
        );
        Assert.Equal(read ? 200 : 404, (await listing.Handle(new(project.Id), Token)).StatusCode);
        var metadata = new GetProjectSingleAudioHandler(
            access,
            audioFiles,
            NullLogger<GetProjectSingleAudioHandler>.Instance
        );
        Assert.Equal(
            read ? 200 : 404,
            (await metadata.Handle(new(project.Id, audio.Id), Token)).StatusCode
        );
        var getAnnotations = new GetAnnotationsByFileHandler(
            access,
            annotations,
            audioFiles,
            NullLogger<GetAnnotationsByFileHandler>.Instance
        );
        Assert.Equal(
            read ? 200 : 404,
            (await getAnnotations.Handle(new(audio.Id), Token)).StatusCode
        );

        var export = new ExportService(projects, audioFiles, annotations);
        Assert.Equal(
            read,
            await export.ExportProjectAsync(project.Id, actor.Object.UserId, Token) is not null
        );
        Assert.Equal(
            read,
            await export.ExportFileAsync(audio.Id, actor.Object.UserId, Token) is not null
        );

        var storage = new Mock<IAudioFileFetchService>();
        storage
            .Setup(x => x.getAudioFileAsync(project.Id, audio.Id, "Wav", Token))
            .ReturnsAsync(
                new AudioFileFetchResultDto(
                    audio.Id,
                    new MemoryStream(),
                    "Wav",
                    "audio/wav",
                    "example.wav"
                )
            );
        var stream = new AudioFileFetchHandler(
            access,
            audioFiles,
            storage.Object,
            projects,
            actor.Object
        );
        Assert.Equal(
            read ? 200 : 404,
            (await stream.Handle(new(project.Id, audio.Id), Token)).StatusCode
        );
        storage.Verify(
            x => x.getAudioFileAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), Token),
            read ? Times.Once() : Times.Never()
        );

        var uploads = new Mock<IAudioFileUploadService>();
        uploads
            .Setup(x =>
                x.UploadBatchAsync(
                    project.Id,
                    It.IsAny<IReadOnlyList<Microsoft.AspNetCore.Http.IFormFile>>(),
                    Token
                )
            )
            .ReturnsAsync(
                new BatchAudioFileUploadResultDto(
                    Guid.NewGuid(),
                    [
                        new AudioFileUploadResultDto(
                            Guid.NewGuid(),
                            "new.wav",
                            "audio/wav",
                            10,
                            Status.Success,
                            null
                        ),
                    ]
                )
            );
        var uploader = new AudioFileUploadHandler(access, projects, uploads.Object, actor.Object);
        // An authorized empty batch reaches input validation; others are stopped by access checks.
        Assert.Equal(
            write ? 400
                : read ? 403
                : 404,
            (await uploader.Handle(new(project.Id, []), Token)).StatusCode
        );
        Assert.Equal(
            write ? 201
                : read ? 403
                : 404,
            (
                await uploader.Handle(
                    new(project.Id, [Mock.Of<Microsoft.AspNetCore.Http.IFormFile>()]),
                    Token
                )
            ).StatusCode
        );
        uploads.Verify(
            x =>
                x.UploadBatchAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<IReadOnlyList<Microsoft.AspNetCore.Http.IFormFile>>(),
                    Token
                ),
            write ? Times.Once() : Times.Never()
        );

        var patchService = new Mock<IAudioFilePatchService>();
        patchService
            .Setup(x =>
                x.patchAudioFileAsync(project.Id, audio.Id, It.IsAny<AudioFilePatchDto>(), Token)
            )
            .ReturnsAsync(new AudioFilePatchResultDto(audio.Id, "renamed.wav"));
        var patchAudio = new AudioFilePatchHandler(
            access,
            audioFiles,
            patchService.Object,
            projects,
            actor.Object
        );
        Assert.Equal(
            write ? 200
                : read ? 403
                : 404,
            (
                await patchAudio.Handle(new(project.Id, audio.Id, new("renamed.wav")), Token)
            ).StatusCode
        );
        patchService.Verify(
            x =>
                x.patchAudioFileAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<AudioFilePatchDto>(),
                    Token
                ),
            write ? Times.Once() : Times.Never()
        );

        var deleteService = new Mock<IAudioFileDeleteService>();
        deleteService
            .Setup(x => x.deleteAudioFileAsync(project.Id, audio.Id, "Wav", Token))
            .ReturnsAsync(new AudioFileDeleteResultDto(Status.Success));
        var deleteAudio = new AudioFileDeleteHandler(
            access,
            audioFiles,
            deleteService.Object,
            projects,
            actor.Object
        );
        Assert.Equal(
            write ? 200
                : read ? 403
                : 404,
            (await deleteAudio.Handle(new(project.Id, audio.Id), Token)).StatusCode
        );
        deleteService.Verify(
            x =>
                x.deleteAudioFileAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    Token
                ),
            write ? Times.Once() : Times.Never()
        );

        var save = new SaveAnnotationHandler(
            access,
            annotations,
            actor.Object,
            NullLogger<SaveAnnotationHandler>.Instance
        );
        var saved = await save.Handle(new("region", audio.Id, ["Test"], 1, 2), Token);
        Assert.Equal(
            write ? 201
                : read ? 403
                : 404,
            saved.StatusCode
        );
        var update = new UpdateAnnotationHandler(
            access,
            annotations,
            actor.Object,
            NullLogger<UpdateAnnotationHandler>.Instance
        );
        Assert.Equal(
            write ? 200
                : read ? 403
                : 404,
            (await update.Handle(new(annotation.Id, audio.Id, ["Changed"], 0, 2), Token)).StatusCode
        );
        var delete = new DeleteAnnotationHandler(
            access,
            annotations,
            NullLogger<DeleteAnnotationHandler>.Instance
        );
        Assert.Equal(
            write ? 200
                : read ? 403
                : 404,
            (await delete.Handle(new(annotation.Id, audio.Id), Token)).StatusCode
        );
    }

    [Fact]
    public async Task ForgedResourcePairs_AreRejectedWithoutMutation()
    {
        var otherProject = new Project("Other project", owner.Id);
        var otherAudio = new AudioFile { ProjectId = otherProject.Id, FileName = "other.wav" };
        db.AddRange(otherProject, otherAudio);
        await db.SaveChangesAsync();
        Assert.Equal(
            404,
            (
                await access.CheckAudioAsync(
                    audio.Id,
                    otherProject.Id,
                    ProjectPermission.Read,
                    Token
                )
            )!.Status
        );
        Assert.Equal(
            404,
            (
                await access.CheckAudioAsync(
                    Guid.NewGuid(),
                    project.Id,
                    ProjectPermission.Read,
                    Token
                )
            )!.Status
        );
        Assert.Equal(
            404,
            (
                await access.CheckAnnotationAsync(
                    annotation.Id,
                    otherAudio.Id,
                    ProjectPermission.Write,
                    Token
                )
            )!.Status
        );
        var update = new UpdateAnnotationHandler(
            access,
            annotations,
            actor.Object,
            NullLogger<UpdateAnnotationHandler>.Instance
        );
        Assert.Equal(
            404,
            (
                await update.Handle(new(annotation.Id, otherAudio.Id, ["Changed"], 0, 2), Token)
            ).StatusCode
        );
        Assert.Equal(1, annotation.EndTimeSeconds);
    }

    [Theory]
    [InlineData(ProjectMemberRole.Owner, true, true)]
    [InlineData(ProjectMemberRole.Annotator, true, true)]
    [InlineData(ProjectMemberRole.Viewer, true, false)]
    [InlineData(null, false, false)]
    public async Task Hub_ProtectsGroupJoinsAndMutations(
        ProjectMemberRole? role,
        bool canRead,
        bool canWrite
    )
    {
        if (role != ProjectMemberRole.Owner)
        {
            if (role.HasValue)
                await projects.AddMemberAsync(project.Id, target.Id, role.Value);
            actor.SetupGet(x => x.UserId).Returns(target.Id);
        }
        var groups = new Mock<IGroupManager>();
        var clients = new Mock<IHubCallerClients>();
        clients.Setup(x => x.OthersInGroup(It.IsAny<string>())).Returns(Mock.Of<IClientProxy>());
        clients.Setup(x => x.Group(It.IsAny<string>())).Returns(Mock.Of<IClientProxy>());
        var context = new Mock<HubCallerContext>();
        context.SetupGet(x => x.ConnectionId).Returns("test-connection");
        var hub = new AudioHub(
            access,
            new SaveAnnotationHandler(
                access,
                annotations,
                actor.Object,
                NullLogger<SaveAnnotationHandler>.Instance
            ),
            new UpdateAnnotationHandler(
                access,
                annotations,
                actor.Object,
                NullLogger<UpdateAnnotationHandler>.Instance
            ),
            new DeleteAnnotationHandler(
                access,
                annotations,
                NullLogger<DeleteAnnotationHandler>.Instance
            )
        )
        {
            Context = context.Object,
            Groups = groups.Object,
            Clients = clients.Object,
        };
        if (canRead)
        {
            await hub.JoinAudioGroup(audio.Id.ToString().ToUpperInvariant());
            groups.Verify(
                x =>
                    x.AddToGroupAsync(
                        "test-connection",
                        audio.Id.ToString(),
                        It.IsAny<CancellationToken>()
                    ),
                Times.Once
            );
        }
        else
        {
            await Assert.ThrowsAsync<HubException>(() => hub.JoinAudioGroup(audio.Id.ToString()));
            groups.VerifyNoOtherCalls();
        }
        if (canWrite)
        {
            await hub.SendRegionData(audio.Id.ToString(), "region");
            Assert.NotNull(await hub.SaveAnnotation(new("region", audio.Id, ["Test"], 1, 2)));
            Assert.NotNull(
                await hub.UpdateAnnotation(new(annotation.Id, audio.Id, ["Changed"], 0, 2))
            );
            Assert.True(await hub.DeleteAnnotation(annotation.Id, audio.Id));
        }
        else
        {
            await Assert.ThrowsAsync<HubException>(() =>
                hub.SendRegionData(audio.Id.ToString(), "region")
            );
            await Assert.ThrowsAsync<HubException>(() =>
                hub.SaveAnnotation(new("region", audio.Id, ["Test"], 1, 2))
            );
            await Assert.ThrowsAsync<HubException>(() =>
                hub.UpdateAnnotation(new(annotation.Id, audio.Id, ["Changed"], 0, 2))
            );
            await Assert.ThrowsAsync<HubException>(() =>
                hub.DeleteAnnotation(annotation.Id, audio.Id)
            );
            clients.VerifyNoOtherCalls();
        }
        await Assert.ThrowsAsync<HubException>(() => hub.JoinAudioGroup("invalid"));
        await Assert.ThrowsAsync<HubException>(() =>
            hub.DeleteAnnotation(annotation.Id, Guid.NewGuid())
        );
    }

    [Fact]
    public void Roles_HaveStringJsonContract()
    {
        var request = JsonSerializer.Deserialize<AddProjectMemberRequest>(
            """{"email":"bob@example.com","role":"Viewer"}""",
            new JsonSerializerOptions(JsonSerializerDefaults.Web)
        );
        Assert.Equal(ProjectMemberRole.Viewer, request!.Role);
        Assert.Equal("\"Annotator\"", JsonSerializer.Serialize(ProjectMemberRole.Annotator));
    }
}
