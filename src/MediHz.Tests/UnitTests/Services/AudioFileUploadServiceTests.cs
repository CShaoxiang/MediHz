using System.Text;
using FluentAssertions;
using MediHz.Application.Abstractions.Storage;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Domain.Projects;
using MediHz.Infrastructure.Persistence;
using MediHz.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Services;

public class AudioFileUploadServiceTests : UnitTestBase
{
    private readonly Project _project;
    private readonly Mock<IStorageService> _storageServiceMock = new();
    private readonly MediHzDbContext _dbContext = new(
        new DbContextOptionsBuilder<MediHzDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options
    );

    private AudioFileUploadService _service;

    public AudioFileUploadServiceTests(ITestOutputHelper output)
        : base(output)
    {
        // TODO: Needs valid owner when user functionality is aligned
        _project = new Project("Sample Project Name", Guid.NewGuid());
        _dbContext.Projects.Add(_project);
        _dbContext.SaveChanges();

        _service = new AudioFileUploadService(
            _storageServiceMock.Object,
            _dbContext,
            Logger<AudioFileUploadService>()
        );
    }

    [Fact]
    public async Task UploadBatchAsync_ShouldReturnFailure_WhenFileIsEmpty()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);
        fileMock.Setup(f => f.FileName).Returns("empty.mp3");
        fileMock.Setup(f => f.ContentType).Returns("audio/mp3");

        // Act
        var result = await _service.UploadBatchAsync(_project.Id, [fileMock.Object]);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items[0].Status.Should().Be(Status.Failure);
        result.Items[0].ErrorMessage.Should().Be("File is empty");
    }

    [Fact]
    public async Task UploadBatchAsync_ShouldReturnFailure_WhenFileExceedsMaxDuration()
    {
        // Arrange
        using var wavStream = CreateDummyWavStream(44100, 1801); // 1801 seconds > MaxDuration (1800)

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(wavStream.Length);
        fileMock.Setup(f => f.FileName).Returns("toolong.wav");
        fileMock.Setup(f => f.ContentType).Returns("audio/wav");
        fileMock.Setup(f => f.OpenReadStream()).Returns(wavStream);

        // Act
        var result = await _service.UploadBatchAsync(_project.Id, [fileMock.Object]);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items[0].Status.Should().Be(Status.Failure);
        result.Items[0].ErrorMessage.Should().Be("File is too long");
    }

    [Fact]
    public async Task UploadBatchAsync_ShouldReturnFailure_WhenFileIsUnderMinDuration()
    {
        // Arrange
        using var wavStream = CreateDummyWavStream(44100, 0); // 0 seconds < MinDuration (1)

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(wavStream.Length); // Ensure length > 0 if possible, or stream will handle it.
        fileMock.Setup(f => f.FileName).Returns("tooshort.wav");
        fileMock.Setup(f => f.ContentType).Returns("audio/wav");
        fileMock.Setup(f => f.OpenReadStream()).Returns(wavStream);

        // Act
        var result = await _service.UploadBatchAsync(_project.Id, [fileMock.Object]);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items[0].Status.Should().Be(Status.Failure);
        result.Items[0].ErrorMessage.Should().Be("File is too short");
    }

    [Fact]
    public async Task UploadBatchAsync_ShouldReturnFailure_WhenFileExceedsMaxSampleRate()
    {
        // Arrange
        using var wavStream = CreateDummyWavStream(48001, 10); // 48001 > MaxSampleRate (48000)

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(wavStream.Length);
        fileMock.Setup(f => f.FileName).Returns("highsamplerate.wav");
        fileMock.Setup(f => f.ContentType).Returns("audio/wav");
        fileMock.Setup(f => f.OpenReadStream()).Returns(wavStream);

        // Act
        var result = await _service.UploadBatchAsync(_project.Id, [fileMock.Object]);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items[0].Status.Should().Be(Status.Failure);
        result.Items[0].ErrorMessage.Should().Be("File does not reach maximum sample rate");
    }

    [Fact]
    public async Task UploadBatchAsync_ShouldReturnFailure_WhenFileIsUnderMinSampleRate()
    {
        // Arrange
        using var wavStream = CreateDummyWavStream(3999, 10); // 3999 < MinSampleRate (4000)

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(wavStream.Length);
        fileMock.Setup(f => f.FileName).Returns("lowsamplerate.wav");
        fileMock.Setup(f => f.ContentType).Returns("audio/wav");
        fileMock.Setup(f => f.OpenReadStream()).Returns(wavStream);

        // Act
        var result = await _service.UploadBatchAsync(_project.Id, [fileMock.Object]);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items[0].Status.Should().Be(Status.Failure);
        result.Items[0].ErrorMessage.Should().Be("File does not reach minimum sample rate");
    }

    [Fact]
    public async Task UploadBatchAsync_ShouldReturnFailure_WhenFileExceedsMaxFileSize()
    {
        // Arrange
        using var wavStream = CreateDummyWavStream(44100, 10);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(350L * 1024 * 1024 + 1); // > MaxFileSize
        fileMock.Setup(f => f.FileName).Returns("toolarge.wav");
        fileMock.Setup(f => f.ContentType).Returns("audio/wav");
        fileMock.Setup(f => f.OpenReadStream()).Returns(wavStream);

        // Act
        var result = await _service.UploadBatchAsync(_project.Id, [fileMock.Object]);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items[0].Status.Should().Be(Status.Failure);
        result.Items[0].ErrorMessage.Should().Be("File is too large");
    }

    [Fact]
    public async Task UploadBatchAsync_ShouldReturnSuccess_WhenFileIsValid()
    {
        // Arrange
        using var wavStream = CreateDummyWavStream(44100, 10); // 10 seconds is valid

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(wavStream.Length);
        fileMock.Setup(f => f.FileName).Returns("valid.wav");
        fileMock.Setup(f => f.ContentType).Returns("audio/wav");
        fileMock.Setup(f => f.OpenReadStream()).Returns(wavStream);

        _storageServiceMock
            .Setup(s =>
                s.SaveFileAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync("exampleProjectId/newfile.wav");

        // Act
        var result = await _service.UploadBatchAsync(_project.Id, [fileMock.Object]);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items[0].Status.Should().Be(Status.Success);
        result.Items[0].ErrorMessage.Should().BeNull();

        // Verify DbContext was updated
        _dbContext.AudioFiles.Count().Should().Be(1);
    }

    private static MemoryStream CreateDummyWavStream(int sampleRate, int durationSeconds)
    {
        var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms, Encoding.UTF8, true);

        short channels = 1;
        short bitsPerSample = 16;
        int subChunk2Size = sampleRate * channels * (bitsPerSample / 8) * durationSeconds;
        int chunkSize = 36 + subChunk2Size;

        writer.Write(Encoding.UTF8.GetBytes("RIFF"));
        writer.Write(chunkSize);
        writer.Write(Encoding.UTF8.GetBytes("WAVE"));
        writer.Write(Encoding.UTF8.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);
        writer.Write(channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * (bitsPerSample / 8));
        writer.Write((short)(channels * (bitsPerSample / 8)));
        writer.Write(bitsPerSample);
        writer.Write(Encoding.UTF8.GetBytes("data"));
        writer.Write(subChunk2Size);

        writer.Write(new byte[subChunk2Size]);

        ms.Position = 0;
        return ms;
    }
}
