using FluentAssertions;
using MediHz.Infrastructure.Services;
using System.IO;
using System.Text;

namespace MediHz.Tests.UnitTests.Services;

public class LocalStorageServiceTests() : IDisposable
{
    private readonly LocalStorageService _service = new();
    private readonly string _testBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MediHz", "Storage");
    private readonly string _testRunId = Guid.NewGuid().ToString();

    public void Dispose()
    {
        // Cleanup only the files created by these tests
        var runFolder = Path.Combine(_testBasePath, _testRunId);
        if (Directory.Exists(runFolder))
        {
            Directory.Delete(runFolder, true);
        }
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
