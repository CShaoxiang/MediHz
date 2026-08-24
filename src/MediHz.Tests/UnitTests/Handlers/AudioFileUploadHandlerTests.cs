using FluentAssertions;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.Handlers;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MediHz.Tests.UnitTests.Handlers;

public class AudioFileUploadHandlerTests()
{
    private readonly Mock<IAudioFileUploadService> _audioFileUploadServiceMock = new();
    private AudioFileUploadHandler _handler => new(_audioFileUploadServiceMock.Object);

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenNoFilesAreProvided()
    {
        // Arrange
        var request = new AudioFileUploadRequest(new List<IFormFile>());

        // Act
        var result = await _handler.Handle(request);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Problem.Should().NotBeNull();
        result.Problem!.Title.Should().Be("No files uploaded");
    }

    [Fact]
    public async Task Handle_ShouldReturnCreated_WhenAllFilesSucceed()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var files = new List<IFormFile> { mockFile.Object };
        var request = new AudioFileUploadRequest(files);
        
        var batchResult = new BatchAudioFileUploadResultDto(Guid.NewGuid(), new List<AudioFileUploadResultDto>
        {
            new AudioFileUploadResultDto(Guid.NewGuid(), "test.mp3", "audio/mp3", 1000, Status.Success, null)
        });

        _audioFileUploadServiceMock
            .Setup(s => s.UploadBatchAsync(files, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batchResult);

        // Act
        var result = await _handler.Handle(request);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Result.Should().BeEquivalentTo(batchResult);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnMultiStatus_WhenSomeFilesFail()
    {
        // Arrange
        var files = new List<IFormFile> { new Mock<IFormFile>().Object, new Mock<IFormFile>().Object };
        var request = new AudioFileUploadRequest(files);
        
        var batchResult = new BatchAudioFileUploadResultDto(Guid.NewGuid(), new List<AudioFileUploadResultDto>
        {
            new AudioFileUploadResultDto(Guid.NewGuid(), "test1.mp3", "audio/mp3", 1000, Status.Success, null),
            new AudioFileUploadResultDto(Guid.NewGuid(), "test2.mp3", "audio/mp3", 0, Status.Failure, "error")
        });

        _audioFileUploadServiceMock
            .Setup(s => s.UploadBatchAsync(files, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batchResult);

        // Act
        var result = await _handler.Handle(request);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status207MultiStatus);
        result.Result.Should().BeEquivalentTo(batchResult);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenAllFilesFail()
    {
        // Arrange
        var files = new List<IFormFile> { new Mock<IFormFile>().Object };
        var request = new AudioFileUploadRequest(files);
        
        var batchResult = new BatchAudioFileUploadResultDto(Guid.NewGuid(), new List<AudioFileUploadResultDto>
        {
            new AudioFileUploadResultDto(Guid.NewGuid(), "test.mp3", "audio/mp3", 0, Status.Failure, "error")
        });

        _audioFileUploadServiceMock
            .Setup(s => s.UploadBatchAsync(files, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batchResult);

        // Act
        var result = await _handler.Handle(request);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Result.Should().BeEquivalentTo(batchResult);
    }
}
