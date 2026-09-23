using System.Text;
using FluentAssertions;
using MediHz.Infrastructure.Services;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Services;

public class LocalStorageServiceTests(ITestOutputHelper output) : UnitTestBase(output)
{
    private readonly LocalStorageService _service = new();
    private readonly string _testBasePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MediHz",
        "Storage"
    );
    private readonly string _testRunId = Guid.NewGuid().ToString();

    public override async Task DisposeAsync()
    {
        // Cleanup only the files created by these tests
        var runFolder = Path.Combine(_testBasePath, _testRunId);
        if (Directory.Exists(runFolder))
        {
            Directory.Delete(runFolder, true);
        }

        await base.DisposeAsync();
    }

    [Fact]
    public async Task SaveFileAsync_ShouldSaveFileToDiskAndReturnRelativePath()
    {
        // Arrange
        var relativePath = $"{_testRunId}/test-file.txt";
        var content = "Hello, Storage!";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = await _service.SaveFileAsync(stream, relativePath);

        // Assert
        result.Should().Be($"{_testRunId}/test-file.txt");
        var fullPath = Path.Combine(_testBasePath, relativePath);
        File.Exists(fullPath).Should().BeTrue();
        var savedContent = await File.ReadAllTextAsync(fullPath);
        savedContent.Should().Be(content);
    }

    [Fact]
    public async Task GetFileAsync_ShouldReturnNull_WhenFileDoesNotExist()
    {
        // Arrange
        var relativePath = $"{_testRunId}/nonexistent/file.txt";

        // Act
        var stream = await _service.GetFileAsync(relativePath);

        // Assert
        stream.Should().BeNull();
    }

    [Fact]
    public async Task GetFileAsync_ShouldReturnStream_WhenFileExists()
    {
        // Arrange
        var relativePath = $"{_testRunId}/fetch-file.txt";
        var content = "Fetch me";
        var fullPath = Path.Combine(_testBasePath, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await File.WriteAllTextAsync(fullPath, content);

        // Act
        var stream = await _service.GetFileAsync(relativePath);

        // Assert
        stream.Should().NotBeNull();
        using var reader = new StreamReader(stream!);
        var result = await reader.ReadToEndAsync();
        result.Should().Be(content);
    }

    [Fact]
    public async Task GetAudioFileAsync_ShouldReturnSameAudio_WhenFileExists()
    {
        // Arrange
        var relativePath = $"{_testRunId}/audio-file.wav";
        byte[] audioContent =
        [
            0x52, 0x49, 0x46, 0x46, // RIFF
            0x24, 0x00, 0x00, 0x00,
            0x57, 0x41, 0x56, 0x45, // WAVE
            0x66, 0x6D, 0x74, 0x20, // fmt
            0x10, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x01, 0x00,
            0x44, 0xAC, 0x00, 0x00,
            0x88, 0x58, 0x01, 0x00,
            0x02, 0x00, 0x10, 0x00,
            0x64, 0x61, 0x74, 0x61, // data
            0x00, 0x00, 0x00, 0x00
        ];
        await using var audioStream = new MemoryStream(audioContent);

        // Act
        await _service.SaveFileAsync(audioStream, relativePath);
        await using var retrievedStream = await _service.GetFileAsync(relativePath);

        // Assert
        retrievedStream.Should().NotBeNull();
        using var memoryStream = new MemoryStream();
        await retrievedStream!.CopyToAsync(memoryStream);
        memoryStream.ToArray().Should().Equal(audioContent);
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldRemoveFileFromDisk()
    {
        // Arrange
        var relativePath = $"{_testRunId}/delete-file.txt";
        var fullPath = Path.Combine(_testBasePath, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await File.WriteAllTextAsync(fullPath, "Delete me");

        File.Exists(fullPath).Should().BeTrue();

        // Act
        await _service.DeleteFileAsync(relativePath);

        // Assert
        File.Exists(fullPath).Should().BeFalse();
    }
}
