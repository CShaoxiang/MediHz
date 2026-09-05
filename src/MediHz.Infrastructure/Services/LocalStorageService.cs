using System.IO;
using MediHz.Application.Abstractions.Storage;
using Microsoft.Extensions.Configuration;

namespace MediHz.Infrastructure.Services;

public class LocalStorageService : IStorageService
{
    private readonly string _basePath;

    public LocalStorageService()
    {
        _basePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MediHz",
            "Storage"
        );
        ;

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<string> SaveFileAsync(
        Stream content,
        string relativePath,
        CancellationToken cancellationToken = default
    )
    {
        var fullPath = Path.Combine(_basePath, relativePath);
        // Ensure target directory exists
        var targetDirectory = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory!);
        }

        //Copy file over
        await using var fileStream = new FileStream(
            fullPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true
        );
        await content.CopyToAsync(fileStream, cancellationToken);

        //Use storage path as storageKey
        return relativePath.Replace("\\", "/");
    }

    public Task<Stream?> GetFileAsync(
        string relativePath,
        CancellationToken cancellationToken = default
    )
    {
        var fullPath = Path.Combine(_basePath, relativePath);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream?>(null);
        }
        // Open the file with FileShare.Read so it doesn't get locked exclusively
        var stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true
        );
        return Task.FromResult<Stream?>(stream);
    }

    public Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        return Task.CompletedTask;
    }
}
