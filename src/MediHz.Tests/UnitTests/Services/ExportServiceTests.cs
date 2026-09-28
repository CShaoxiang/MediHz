using AutoFixture;
using FluentAssertions;
using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Annotations;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.DTOs.Projects;
using MediHz.Domain.Audio;
using MediHz.Infrastructure.Services;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Services;

public class ExportServiceTests : UnitTestBase
{
    private readonly Mock<IProjectService> _projectServiceMock = new();
    private readonly Mock<IAudioFileService> _audioFileServiceMock = new();
    private readonly Mock<IAnnotationService> _annotationServiceMock = new();
    private readonly ExportService _exportService;

    public ExportServiceTests(ITestOutputHelper output)
        : base(output)
    {
        _exportService = new(
            _projectServiceMock.Object,
            _audioFileServiceMock.Object,
            _annotationServiceMock.Object
        );
    }

    [Fact]
    public async Task ExportProjectAsync_ShouldProcessFilesSequentially_AndReturnProjectExportDto()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = Fixture.Create<ProjectDto>() with { Id = projectId };
        var members = Fixture.CreateMany<ProjectMemberDto>(2).ToArray();

        var file1 = Fixture.Create<AudioFileDto>() with { ProjectId = projectId };
        var file2 = Fixture.Create<AudioFileDto>() with { ProjectId = projectId };
        var files = new[] { file1, file2 };

        _projectServiceMock
            .Setup(x => x.GetByIdAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _projectServiceMock
            .Setup(x => x.GetMembersAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(members);
        _audioFileServiceMock
            .Setup(x => x.GetForProjectAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(files);

        _audioFileServiceMock
            .Setup(x => x.GetByIdAsync(file1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file1);
        _audioFileServiceMock
            .Setup(x => x.GetByIdAsync(file2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file2);

        _annotationServiceMock
            .Setup(x => x.GetForAudioFileAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _exportService.ExportProjectAsync(
            projectId,
            userId,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        result!.ProjectId.Should().Be(projectId);
        result.ProjectName.Should().Be(project.Name);
        result.AnnotatorIds.Should().BeEquivalentTo(members.Select(m => m.UserId));
        result.Files.Should().HaveCount(2);
        result.Files.Select(f => f.FileId).Should().Contain([file1.Id, file2.Id]);
    }

    [Fact]
    public async Task ExportProjectAsync_ShouldReturnNull_WhenProjectDoesNotExistOrAccessDenied()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _projectServiceMock
            .Setup(x => x.GetByIdAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectDto?)null);

        // Act
        var result = await _exportService.ExportProjectAsync(
            projectId,
            userId,
            CancellationToken.None
        );

        // Assert
        result.Should().BeNull();
        _projectServiceMock.Verify(
            x => x.GetMembersAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _audioFileServiceMock.Verify(
            x => x.GetForProjectAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ExportProjectAsync_ShouldReturnEmptyFiles_WhenProjectHasNoFiles()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = Fixture.Create<ProjectDto>() with { Id = projectId };
        var members = Fixture.CreateMany<ProjectMemberDto>(2).ToArray();

        _projectServiceMock
            .Setup(x => x.GetByIdAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _projectServiceMock
            .Setup(x => x.GetMembersAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(members);
        _audioFileServiceMock
            .Setup(x => x.GetForProjectAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _exportService.ExportProjectAsync(
            projectId,
            userId,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        result!.Files.Should().BeEmpty();
        result.AnnotatorIds.Should().BeEquivalentTo(members.Select(m => m.UserId));
    }

    [Fact]
    public async Task ExportProjectAsync_ShouldReturnEmptyAnnotatorIds_WhenProjectHasNoMembers()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = Fixture.Create<ProjectDto>() with { Id = projectId };

        _projectServiceMock
            .Setup(x => x.GetByIdAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _projectServiceMock
            .Setup(x => x.GetMembersAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _audioFileServiceMock
            .Setup(x => x.GetForProjectAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _exportService.ExportProjectAsync(
            projectId,
            userId,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        result!.AnnotatorIds.Should().BeEmpty();
    }

    [Fact]
    public async Task ExportProjectAsync_ShouldSkipFile_WhenIndividualFileExportReturnsNull()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = Fixture.Create<ProjectDto>() with { Id = projectId };
        var members = Fixture.CreateMany<ProjectMemberDto>(1).ToArray();

        var file1 = Fixture.Create<AudioFileDto>() with { ProjectId = projectId };
        var file2 = Fixture.Create<AudioFileDto>() with { ProjectId = projectId };

        _projectServiceMock
            .Setup(x => x.GetByIdAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _projectServiceMock
            .Setup(x => x.GetMembersAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(members);
        _audioFileServiceMock
            .Setup(x => x.GetForProjectAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([file1, file2]);

        // file1 exists and exports successfully
        _audioFileServiceMock
            .Setup(x => x.GetByIdAsync(file1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file1);
        _annotationServiceMock
            .Setup(x => x.GetForAudioFileAsync(file1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // file2 returns null when fetching (e.g., deleted concurrently)
        _audioFileServiceMock
            .Setup(x => x.GetByIdAsync(file2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AudioFileDto?)null);

        // Act
        var result = await _exportService.ExportProjectAsync(
            projectId,
            userId,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        result!.Files.Should().ContainSingle();
        result.Files[0].FileId.Should().Be(file1.Id);
    }

    [Fact]
    public async Task ExportProjectAsync_ShouldPropagateCancellationToken()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = Fixture.Create<ProjectDto>() with { Id = projectId };
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _projectServiceMock
            .Setup(x => x.GetByIdAsync(projectId, userId, token))
            .ReturnsAsync(project);
        _projectServiceMock.Setup(x => x.GetMembersAsync(projectId, token)).ReturnsAsync([]);
        _audioFileServiceMock.Setup(x => x.GetForProjectAsync(projectId, token)).ReturnsAsync([]);

        // Act
        var result = await _exportService.ExportProjectAsync(projectId, userId, token);

        // Assert
        result.Should().NotBeNull();
        _projectServiceMock.Verify(x => x.GetByIdAsync(projectId, userId, token), Times.Once);
        _projectServiceMock.Verify(x => x.GetMembersAsync(projectId, token), Times.Once);
        _audioFileServiceMock.Verify(x => x.GetForProjectAsync(projectId, token), Times.Once);
    }

    [Fact]
    public async Task ExportProjectAsync_ShouldPropagateException_WhenProjectServiceThrows()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _projectServiceMock
            .Setup(x => x.GetByIdAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Project query error"));

        // Act
        var act = () =>
            _exportService.ExportProjectAsync(projectId, userId, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Project query error");
    }

    [Fact]
    public async Task ExportFileAsync_ShouldReturnFileExportDto_WhenFileAndProjectExistWithAnnotations()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var file = Fixture.Create<AudioFileDto>() with
        {
            Id = fileId,
            ProjectId = projectId,
            Format = AudioFileFormat.Wav,
        };
        var project = Fixture.Create<ProjectDto>() with { Id = projectId };
        var annotation = Fixture.Create<AnnotationDto>() with
        {
            AudioFileId = fileId,
            LabelNames = new List<string> { "Test Label" },
        };

        _audioFileServiceMock
            .Setup(x => x.GetByIdAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        _projectServiceMock
            .Setup(x => x.GetByIdAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _annotationServiceMock
            .Setup(x => x.GetForAudioFileAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([annotation]);

        // Act
        var result = await _exportService.ExportFileAsync(fileId, userId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.FileId.Should().Be(fileId);
        result.FileName.Should().Be(file.FileName);
        result.FileType.Should().Be(file.Format.ToString());
        result.DurationSeconds.Should().Be(file.Duration);
        result.SampleRate.Should().Be(file.SampleRate);
        result.Channels.Should().Be(file.Channels);
        result.Annotations.Should().HaveCount(1);

        var exportedAnnotation = result.Annotations[0];
        exportedAnnotation.AnnotationId.Should().Be(annotation.Id);
        exportedAnnotation.Label.Should().Be("Test Label");
        exportedAnnotation.StartTime.Should().Be(annotation.StartTimeSeconds);
        exportedAnnotation.EndTime.Should().Be(annotation.EndTimeSeconds);
        exportedAnnotation.AnnotatorId.Should().Be(annotation.AuthorId);
    }

    [Fact]
    public async Task ExportFileAsync_ShouldReturnNull_WhenAudioFileDoesNotExist()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _audioFileServiceMock
            .Setup(x => x.GetByIdAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AudioFileDto?)null);

        // Act
        var result = await _exportService.ExportFileAsync(fileId, userId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _projectServiceMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _annotationServiceMock.Verify(
            x => x.GetForAudioFileAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ExportFileAsync_ShouldReturnNull_WhenProjectDoesNotExistOrAccessDenied()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var file = Fixture.Create<AudioFileDto>() with { Id = fileId, ProjectId = projectId };

        _audioFileServiceMock
            .Setup(x => x.GetByIdAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        _projectServiceMock
            .Setup(x => x.GetByIdAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectDto?)null);

        // Act
        var result = await _exportService.ExportFileAsync(fileId, userId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _annotationServiceMock.Verify(
            x => x.GetForAudioFileAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ExportFileAsync_ShouldReturnEmptyAnnotations_WhenNoAnnotationsExistForAudioFile()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var file = Fixture.Create<AudioFileDto>() with { Id = fileId, ProjectId = projectId };
        var project = Fixture.Create<ProjectDto>() with { Id = projectId };

        _audioFileServiceMock
            .Setup(x => x.GetByIdAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        _projectServiceMock
            .Setup(x => x.GetByIdAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _annotationServiceMock
            .Setup(x => x.GetForAudioFileAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _exportService.ExportFileAsync(fileId, userId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Annotations.Should().BeEmpty();
    }

    [Fact]
    public async Task ExportFileAsync_ShouldFallbackToPlaceholderLabel_WhenAnnotationLabelNameIsNull()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var file = Fixture.Create<AudioFileDto>() with { Id = fileId, ProjectId = projectId };
        var project = Fixture.Create<ProjectDto>() with { Id = projectId };
        var annotation = Fixture.Create<AnnotationDto>() with
        {
            AudioFileId = fileId,
            LabelNames = new List<string>(),
        };

        _audioFileServiceMock
            .Setup(x => x.GetByIdAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        _projectServiceMock
            .Setup(x => x.GetByIdAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _annotationServiceMock
            .Setup(x => x.GetForAudioFileAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([annotation]);

        // Act
        var result = await _exportService.ExportFileAsync(fileId, userId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Annotations.Should().ContainSingle();
        result.Annotations[0].Label.Should().Be("placeholder label");
    }

    [Fact]
    public async Task ExportFileAsync_ShouldPropagateCancellationToken()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var file = Fixture.Create<AudioFileDto>() with { Id = fileId, ProjectId = projectId };
        var project = Fixture.Create<ProjectDto>() with { Id = projectId };
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _audioFileServiceMock.Setup(x => x.GetByIdAsync(fileId, token)).ReturnsAsync(file);
        _projectServiceMock
            .Setup(x => x.GetByIdAsync(projectId, userId, token))
            .ReturnsAsync(project);
        _annotationServiceMock.Setup(x => x.GetForAudioFileAsync(fileId, token)).ReturnsAsync([]);

        // Act
        var result = await _exportService.ExportFileAsync(fileId, userId, token);

        // Assert
        result.Should().NotBeNull();
        _audioFileServiceMock.Verify(x => x.GetByIdAsync(fileId, token), Times.Once);
        _projectServiceMock.Verify(x => x.GetByIdAsync(projectId, userId, token), Times.Once);
        _annotationServiceMock.Verify(x => x.GetForAudioFileAsync(fileId, token), Times.Once);
    }

    [Fact]
    public async Task ExportFileAsync_ShouldPropagateException_WhenAudioFileServiceThrows()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _audioFileServiceMock
            .Setup(x => x.GetByIdAsync(fileId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("AudioFile retrieval error"));

        // Act
        var act = () => _exportService.ExportFileAsync(fileId, userId, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("AudioFile retrieval error");
    }
}
