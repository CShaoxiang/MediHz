using FluentAssertions;
using MediHz.Domain.Annotations;
using MediHz.Domain.Audio;
using MediHz.Domain.Projects;
using MediHz.Infrastructure.Persistence;
using MediHz.Infrastructure.Persistence.Services.Projects;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Services;

public class ProjectServiceAccessTests : UnitTestBase
{
    private readonly MediHzDbContext _dbContext = new(
        new DbContextOptionsBuilder<MediHzDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options
    );

    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Guid _memberId = Guid.NewGuid();
    private readonly Guid _outsiderId = Guid.NewGuid();
    private readonly AudioFile _audioFile;
    private readonly AudioFile _otherAudioFile;
    private readonly Annotation _annotation;

    private readonly ProjectService _service;

    public ProjectServiceAccessTests(ITestOutputHelper output)
        : base(output)
    {
        var project = new Project("Project", _ownerId);
        project.Members.Add(
            new ProjectMember
            {
                ProjectId = project.Id,
                UserId = _memberId,
                Role = ProjectMemberRole.Annotator,
            }
        );
        _audioFile = new AudioFile { FileName = "a.mp3", ProjectId = project.Id };
        _otherAudioFile = new AudioFile { FileName = "b.mp3", ProjectId = project.Id };
        _annotation = new Annotation
        {
            AudioFileId = _audioFile.Id,
            UserId = _ownerId,
            StartTimeSeconds = 0,
            EndTimeSeconds = 1,
            DurationSeconds = 1,
            UpdatedAtUtc = DateTimeOffset.UtcNow,
        };

        _dbContext.Projects.Add(project);
        _dbContext.AudioFiles.AddRange(_audioFile, _otherAudioFile);
        _dbContext.Annotations.Add(_annotation);
        _dbContext.SaveChanges();

        _service = new ProjectService(_dbContext);
    }

    [Fact]
    public async Task CanAccessAudioFileAsync_ShouldAllowOwnerAndMembers()
    {
        (await _service.CanAccessAudioFileAsync(_audioFile.Id, _ownerId)).Should().BeTrue();
        (await _service.CanAccessAudioFileAsync(_audioFile.Id, _memberId)).Should().BeTrue();
    }

    [Fact]
    public async Task CanAccessAudioFileAsync_ShouldDenyNonMembers()
    {
        (await _service.CanAccessAudioFileAsync(_audioFile.Id, _outsiderId)).Should().BeFalse();
    }

    [Fact]
    public async Task CanAccessAnnotationAsync_ShouldAllowMembers()
    {
        (await _service.CanAccessAnnotationAsync(_annotation.Id, _audioFile.Id, _memberId))
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task CanAccessAnnotationAsync_ShouldDenyNonMembers()
    {
        (await _service.CanAccessAnnotationAsync(_annotation.Id, _audioFile.Id, _outsiderId))
            .Should()
            .BeFalse();
    }

    [Fact]
    public async Task CanAccessAnnotationAsync_ShouldDeny_WhenAudioFileDoesNotMatch()
    {
        (await _service.CanAccessAnnotationAsync(_annotation.Id, _otherAudioFile.Id, _memberId))
            .Should()
            .BeFalse();
    }
}
