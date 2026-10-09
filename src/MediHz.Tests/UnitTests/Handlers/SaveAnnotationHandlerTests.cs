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

public class SaveAnnotationHandlerTests : UnitTestBase
{
    private readonly Mock<IAnnotationService> _annotationServiceMock = new();
    private readonly Mock<IProjectService> _projectServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private SaveAnnotationHandler _handler;

    public SaveAnnotationHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _currentUserMock.Setup(c => c.UserId).Returns(Guid.Empty);
        _projectServiceMock
            .Setup(s =>
                s.CanAccessAudioFileAsync(
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
            Logger<SaveAnnotationHandler>()
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnCreated_WhenAnnotationIsCreatedSuccessfully()
    {
        // Arrange
        var request = Fixture.Create<SaveAnnotationCommand>();
        var annotationDto = Fixture.Create<AnnotationDto>();

        _annotationServiceMock
            .Setup(s =>
                s.CreateAsync(
                    Guid.Empty,
                    It.Is<CreateAnnotationRequest>(r =>
                        r.Id == request.AnnotationId
                        && r.AudioFileId == request.AudioFileId
                        && r.LabelNames.SequenceEqual(request.LabelNames)
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
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().BeEquivalentTo(annotationDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var request = Fixture.Create<SaveAnnotationCommand>();
        var exceptionMessage = "Something went wrong";

        _annotationServiceMock
            .Setup(s =>
                s.CreateAsync(
                    Guid.Empty,
                    It.IsAny<CreateAnnotationRequest>(),
                    CancellationToken.None
                )
            )
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("Annotation Creation Failed");
        result.Problem.Detail.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenUserIsNotProjectMember()
    {
        // Arrange
        var request = Fixture.Create<SaveAnnotationCommand>();

        _projectServiceMock
            .Setup(s =>
                s.CanAccessAudioFileAsync(request.AudioFileId, Guid.Empty, CancellationToken.None)
            )
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        _annotationServiceMock.Verify(
            s =>
                s.CreateAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CreateAnnotationRequest>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }
}
