using AutoFixture;
using FluentAssertions;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.Handlers.Projects;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class GetProjectAudioFilesHandlerTests : UnitTestBase
{
    private readonly Mock<IAudioFileService> _audioFileService = new();

    private GetProjectAudioFilesHandler _handler;

    public GetProjectAudioFilesHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _handler = new(
            Mock.Of<MediHz.Application.Abstractions.Projects.IProjectAccessService>(),
            _audioFileService.Object,
            Logger<GetProjectAudioFilesHandler>()
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnAudioFile_WhenOneExistsWithProject()
    {
        // Arrange
        var request = new GetProjectAudioFilesRequest(Guid.NewGuid());
        var audioFile = Fixture.Create<AudioFileDto>();
        _audioFileService
            .Setup(a => a.GetForProjectAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync([audioFile]);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task Handle_ShouldReturnAudioFiles_WhenMultipleExistWithProject()
    {
        // Arrange
        var request = new GetProjectAudioFilesRequest(Guid.NewGuid());
        var audioFiles = Fixture.CreateMany<AudioFileDto>(3).ToArray();
        _audioFileService
            .Setup(a => a.GetForProjectAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(audioFiles);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().HaveCount(3);
        result.Value.Should().BeEquivalentTo(audioFiles);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyListSuccess_WhenNoAudioFilesExistWithProject()
    {
        // Arrange
        var request = new GetProjectAudioFilesRequest(Guid.NewGuid());
        _audioFileService
            .Setup(a => a.GetForProjectAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEmpty();
    }
}
