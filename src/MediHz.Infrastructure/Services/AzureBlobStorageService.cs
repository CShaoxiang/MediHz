using Azure;
using Azure.Storage.Blobs;
using MediHz.Application.Abstractions.Storage;

namespace MediHz.Infrastructure.Services;

public class AzureBlobStorageService(BlobContainerClient container) : IStorageService
{
    public const string ContainerName = "audio";

    public async Task<string> SaveFileAsync(
        Stream content,
        string relativePath,
        CancellationToken cancellationToken = default
    )
    {
        var blobName = ToBlobName(relativePath);
        await container
            .GetBlobClient(blobName)
            .UploadAsync(content, overwrite: true, cancellationToken);
        return blobName;
    }

    public async Task<Stream?> GetFileAsync(
        string relativePath,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var stream = await container
                .GetBlobClient(ToBlobName(relativePath))
                .OpenReadAsync(cancellationToken: cancellationToken);
            return new NoTimeoutStream(stream);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task DeleteFileAsync(
        string relativePath,
        CancellationToken cancellationToken = default
    )
    {
        await container
            .GetBlobClient(ToBlobName(relativePath))
            .DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    private static string ToBlobName(string relativePath) => relativePath.Replace("\\", "/");
}
