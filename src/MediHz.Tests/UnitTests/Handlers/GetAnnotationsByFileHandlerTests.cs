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

public class GetAnnotationsByFileHandlerTests : UnitTestBase
{
    private readonly Mock<IAnnotationService> _annotationServiceMock = new();
    private readonly Mock<IProjectService> _projectServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private GetAnnotationsByFileHandler _handler;

    public GetAnnotationsByFileHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _currentUserMock.Setup(c => c.UserId).Returns(Guid.Empty);
        _handler = new(
            _annotationServiceMock.Object,
            _projectServiceMock.Object,
            _currentUserMock.Object,
            Logger<GetAnnotationsByFileHandler>()
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenUserCanAccessAudioFile()
    {
        // Arrange
        var request = Fixture.Create<GetAnnotationsByFileRequest>();
        var annotations = Fixture.CreateMany<AnnotationDto>().ToList();

        _projectServiceMock
            .Setup(s =>
                s.CanAccessAudioFileAsync(request.AudioFileId, Guid.Empty, CancellationToken.None)
            )
            .ReturnsAsync(true);

        _annotationServiceMock
            .Setup(s => s.GetForAudioFileAsync(request.AudioFileId, CancellationToken.None))
            .ReturnsAsync(annotations);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(annotations);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserCannotAccessAudioFile()
    {
        // Arrange
        var request = Fixture.Create<GetAnnotationsByFileRequest>();

        _projectServiceMock
            .Setup(s =>
                s.CanAccessAudioFileAsync(request.AudioFileId, Guid.Empty, CancellationToken.None)
            )
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("File not found");
    }
}
