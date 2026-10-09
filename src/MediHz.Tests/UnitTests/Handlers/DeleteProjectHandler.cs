using FluentAssertions;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Projects;
using MediHz.Application.Handlers.Audio;
using MediHz.Application.Handlers.Projects;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;

public class DeleteProjectHandlerTests : UnitTestBase
{
    private readonly Mock<IProjectService> _projectServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private readonly Mock<IAudioFileService> _audioFileServiceMock = new();
    private readonly Mock<IAudioFileDeleteHandler> _audioFileDeleteHandlerMock = new();
    private readonly Guid _userId = Guid.NewGuid();

    private DeleteProjectHandler _handler;

    public DeleteProjectHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _currentUserMock.Setup(cu => cu.UserId).Returns(_userId);
        _handler = new DeleteProjectHandler(
            Mock.Of<IProjectAccessService>(),
            _currentUserMock.Object,
            _projectServiceMock.Object,
            _audioFileServiceMock.Object,
            _audioFileDeleteHandlerMock.Object,
            Logger<DeleteProjectHandler>()
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProjectNotFound()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new ProjectDeleteRequest(projectId, _userId);
        _projectServiceMock
            .Setup(s => s.GetByIdAsync(projectId, _userId, CancellationToken.None))
            .ReturnsAsync((ProjectDto)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProjectIsDeleted()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        // Add a project to the mock service to simulate that it exists

        _projectServiceMock
            .Setup(s => s.GetByIdAsync(projectId, _userId, CancellationToken.None))
            .ReturnsAsync(
                new ProjectDto(projectId, "Test Project", _userId, "Owner", DateTime.UtcNow)
            );

        var request = new ProjectDeleteRequest(projectId, _userId);
        _projectServiceMock
            .Setup(s => s.DeleteAsync(projectId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenWrongUserId()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var wrongUserId = Guid.NewGuid();
        var request = new ProjectDeleteRequest(projectId, wrongUserId);
        _projectServiceMock
            .Setup(s => s.GetByIdAsync(projectId, wrongUserId, CancellationToken.None))
            .ReturnsAsync((ProjectDto)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldDeleteAudio_WhenProjectIsDeleted()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new ProjectDeleteRequest(projectId, _userId);
        _projectServiceMock
            .Setup(s => s.GetByIdAsync(projectId, _userId, CancellationToken.None))
            .ReturnsAsync(
                new ProjectDto(projectId, "Test Project", _userId, "Owner", DateTime.UtcNow)
            );

        _projectServiceMock
            .Setup(s => s.DeleteAsync(projectId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        _projectServiceMock.Verify(
            s => s.DeleteAsync(projectId, CancellationToken.None),
            Times.Once
        );
    }
}
