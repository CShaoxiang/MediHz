using AutoFixture;
using FluentAssertions;
using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.DTOs.Annotations;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.Handlers.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class GetAnnotationsByFileHandlerTests : UnitTestBase
{
    private readonly Mock<IAnnotationService> _annotationServiceMock = new();
    private readonly Mock<IAudioFileService> _audioFileServiceMock = new();

    private GetAnnotationsByFileHandler _handler;

    public GetAnnotationsByFileHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _handler = new(
            _annotationServiceMock.Object,
            _audioFileServiceMock.Object,
            Logger<GetAnnotationsByFileHandler>()
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenAudioFileExists()
    {
        // Arrange
        var request = Fixture.Create<GetAnnotationsByFileRequest>();
        var audioFileDto = Fixture.Create<AudioFileDto>();
        var annotations = Fixture.CreateMany<AnnotationDto>().ToList();

        _audioFileServiceMock
            .Setup(s => s.GetByIdAsync(request.AudioFileId))
            .ReturnsAsync(audioFileDto);

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
    public async Task Handle_ShouldReturnFailure_WhenAudioFileDoesNotExist()
    {
        // Arrange
        var request = Fixture.Create<GetAnnotationsByFileRequest>();

        _audioFileServiceMock
            .Setup(s => s.GetByIdAsync(request.AudioFileId))
            .ReturnsAsync((AudioFileDto?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("File not found");
    }
}
