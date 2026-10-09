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

public class UpdateAnnotationHandlerTests : UnitTestBase
{
    private readonly Mock<IAnnotationService> _annotationServiceMock = new();
    private readonly Mock<IProjectService> _projectServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private UpdateAnnotationHandler _handler;

    public UpdateAnnotationHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _currentUserMock.Setup(c => c.UserId).Returns(Guid.Empty);
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
            Logger<UpdateAnnotationHandler>()
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenAnnotationIsUpdatedSuccessfully()
    {
        // Arrange
        var request = Fixture.Create<UpdateAnnotationCommand>();
        var annotationDto = Fixture.Create<AnnotationDto>();

        _annotationServiceMock
            .Setup(s =>
                s.UpdateAsync(
                    request.AnnotationId,
                    It.Is<UpdateAnnotationRequest>(r =>
                        r.LabelNames.SequenceEqual(request.LabelNames)
                        && r.StartTimeSeconds == request.StartTimeSeconds
                        && r.EndTimeSeconds == request.EndTimeSeconds
                    ),
                    CancellationToken.None
                )
            )
            .ReturnsAsync(annotationDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(annotationDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenAnnotationDoesNotExist()
    {
        // Arrange
        var request = Fixture.Create<UpdateAnnotationCommand>();

        _annotationServiceMock
            .Setup(s =>
                s.UpdateAsync(
                    request.AnnotationId,
                    It.IsAny<UpdateAnnotationRequest>(),
                    CancellationToken.None
                )
            )
            .ReturnsAsync((AnnotationDto?)null);

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
        var request = Fixture.Create<UpdateAnnotationCommand>();
        var exceptionMessage = "Something went wrong";

        _annotationServiceMock
            .Setup(s =>
                s.UpdateAsync(
                    request.AnnotationId,
                    It.IsAny<UpdateAnnotationRequest>(),
                    CancellationToken.None
                )
            )
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("Annotation Update Failed");
        result.Problem.Detail.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenUserCannotAccessAnnotation()
    {
        // Arrange
        var request = Fixture.Create<UpdateAnnotationCommand>();

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
            s =>
                s.UpdateAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<UpdateAnnotationRequest>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }
}
