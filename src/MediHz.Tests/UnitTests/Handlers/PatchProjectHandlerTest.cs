using FluentAssertions;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Projects;
using MediHz.Application.Handlers.Projects;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;

public class PatchProjectHandlerTests : UnitTestBase
{
    private readonly Mock<IProjectService> _projectServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Guid _userId = Guid.NewGuid();

    private PatchProjectHandler _handler;

    public PatchProjectHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _currentUserMock.Setup(cu => cu.UserId).Returns(_userId);
        _handler = new PatchProjectHandler(
            _projectServiceMock.Object,
            Logger<IPatchProjectHandler>()
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProjectNotFound()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var change = new ProjectPatchRequest(
            projectId,
            _userId,
            new UpdateProjectRequest("new name")
        );
        _projectServiceMock
            .Setup(s => s.GetByIdAsync(projectId, _userId, CancellationToken.None))
            .ReturnsAsync((ProjectDto)null);

        // Act
        var result = await _handler.Handle(change, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProjectIsPatched()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var change = new ProjectPatchRequest(
            projectId,
            _userId,
            new UpdateProjectRequest("new name")
        );
        var updatedProject = new ProjectDto(
            projectId,
            "new name",
            _userId,
            "Owner",
            DateTime.UtcNow
        );
        _projectServiceMock
            .Setup(s => s.UpdateAsync(projectId, change.Change, CancellationToken.None))
            .ReturnsAsync(updatedProject);

        // Act
        var result = await _handler.Handle(change, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(updatedProject);
    }
}
