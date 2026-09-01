using FluentAssertions;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.DTOs.Projects;
using MediHz.Application.Handlers.Audio;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class AudioFileUploadHandlerTests : UnitTestBase
{
    private readonly Mock<IProjectService> _projectServiceMock = new();
    private readonly Mock<IAudioFileUploadService> _audioFileUploadServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Guid _userId = Guid.NewGuid();
    private AudioFileUploadHandler _handler => new(
        _projectServiceMock.Object,
        _audioFileUploadServiceMock.Object,
        _currentUserMock.Object);

    public AudioFileUploadHandlerTests(ITestOutputHelper output) : base(output)
    {
        _currentUserMock.SetupGet(user => user.UserId).Returns(_userId);
        _projectServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProjectDto(Guid.NewGuid(), "Test Project", Guid.NewGuid(), "", DateTime.Now));
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenProjectIsNotFound()
    {
        // Arrange
        var request = new AudioFileUploadRequest(Guid.NewGuid(), []);

        _projectServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectDto?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("Project not found");
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenNoFilesAreProvided()
    {
        // Arrange
        var request = new AudioFileUploadRequest(Guid.NewGuid(), []);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("No files uploaded");
    }

    [Fact]
    public async Task Handle_ShouldReturnCreated_WhenAllFilesSucceed()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var files = new List<IFormFile> { mockFile.Object };
        var request = new AudioFileUploadRequest(Guid.NewGuid(), files);

        var batchResult = new BatchAudioFileUploadResultDto(Guid.NewGuid(),
        [
            new AudioFileUploadResultDto(Guid.NewGuid(), "test.mp3", "audio/mp3", 1000, Status.Success, null)
        ]);

        _audioFileUploadServiceMock
            .Setup(s => s.UploadBatchAsync(It.IsAny<Guid>(), It.IsAny<List<IFormFile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(batchResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().BeEquivalentTo(batchResult);
    }

    [Fact]
    public async Task Handle_ShouldReturnMultiStatus_WhenSomeFilesFail()
    {
        // Arrange
        var files = new List<IFormFile> { new Mock<IFormFile>().Object, new Mock<IFormFile>().Object };
        var request = new AudioFileUploadRequest(Guid.NewGuid(), files);

        var batchResult = new BatchAudioFileUploadResultDto(Guid.NewGuid(),
        [
            new AudioFileUploadResultDto(Guid.NewGuid(), "test1.mp3", "audio/mp3", 1000, Status.Success, null),
            new AudioFileUploadResultDto(Guid.NewGuid(), "test2.mp3", "audio/mp3", 0, Status.Failure, "error")
        ]);

        _audioFileUploadServiceMock
            .Setup(s => s.UploadBatchAsync(It.IsAny<Guid>(), It.IsAny<List<IFormFile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(batchResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status207MultiStatus);
        result.Value.Should().BeEquivalentTo(batchResult);
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenAllFilesFail()
    {
        // Arrange
        var files = new List<IFormFile> { new Mock<IFormFile>().Object };
        var request = new AudioFileUploadRequest(Guid.NewGuid(), files);

        var batchResult = new BatchAudioFileUploadResultDto(Guid.NewGuid(),
        [
            new AudioFileUploadResultDto(Guid.NewGuid(), "test.mp3", "audio/mp3", 0, Status.Failure, "error")
        ]);

        _audioFileUploadServiceMock
            .Setup(s => s.UploadBatchAsync(It.IsAny<Guid>(), It.IsAny<List<IFormFile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(batchResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}
