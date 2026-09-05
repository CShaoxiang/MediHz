using AutoFixture;
using FluentAssertions;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Projects;
using MediHz.Application.Handlers.Projects;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class CreateProjectHandlerTests : UnitTestBase
{
    private readonly Mock<IProjectService> _projectServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private ProjectDto _projectDto;

    private CreateProjectHandler _handler;

    public CreateProjectHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _projectDto = Fixture.Create<ProjectDto>();
        _projectServiceMock
            .Setup(s =>
                s.CreateAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CreateProjectRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(_projectDto);
        _currentUserMock.SetupGet(user => user.UserId).Returns(_projectDto.OwnerId);

        _handler = new(
            _projectServiceMock.Object,
            _currentUserMock.Object,
            Logger<CreateProjectHandler>()
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnCreated_WhenProjectIsCreated()
    {
        // Arrange
        var request = new CreateProjectRequest("New Project");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().BeEquivalentTo(_projectDto);
        _projectServiceMock.Verify(
            service => service.CreateAsync(_projectDto.OwnerId, request, CancellationToken.None),
            Times.Once
        );
    }
}
