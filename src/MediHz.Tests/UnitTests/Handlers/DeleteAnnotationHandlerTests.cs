using AutoFixture;
using FluentAssertions;
using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Annotations;
using MediHz.Application.Handlers.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class DeleteAnnotationHandlerTests : UnitTestBase
{
    private readonly Mock<IAnnotationService> _annotationServiceMock = new();
    private readonly Mock<IProjectService> _projectServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private DeleteAnnotationHandler _handler;

    public DeleteAnnotationHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _projectServiceMock
            .Setup(s =>
                s.CanAccessAnnotationAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);
        _handler = new(
            Mock.Of<MediHz.Application.Abstractions.Projects.IProjectAccessService>(),
            _annotationServiceMock.Object,
            _projectServiceMock.Object,
            _currentUserMock.Object,
            Logger<DeleteAnnotationHandler>()
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenAnnotationIsDeletedSuccessfully()
    {
        // Arrange
        var request = Fixture.Create<DeleteAnnotationCommand>();

        _annotationServiceMock
            .Setup(s => s.DeleteAsync(request.AnnotationId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenAnnotationDoesNotExist()
    {
        // Arrange
        var request = Fixture.Create<DeleteAnnotationCommand>();

        _annotationServiceMock
            .Setup(s => s.DeleteAsync(request.AnnotationId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("Annotation Not Found");
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var request = Fixture.Create<DeleteAnnotationCommand>();
        var exceptionMessage = "Something went wrong";

        _annotationServiceMock
            .Setup(s => s.DeleteAsync(request.AnnotationId, CancellationToken.None))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("Annotation Deletion Failed");
        result.Problem.Detail.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenUserCannotAccessAnnotation()
    {
        // Arrange
        var request = Fixture.Create<DeleteAnnotationCommand>();

        _projectServiceMock
            .Setup(s =>
                s.CanAccessAnnotationAsync(
                    request.AnnotationId,
                    request.AudioFileId,
                    Guid.Empty,
                    CancellationToken.None
                )
            )
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        _annotationServiceMock.Verify(
            s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
