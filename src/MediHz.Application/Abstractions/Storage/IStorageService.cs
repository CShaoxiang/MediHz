namespace MediHz.Application.Abstractions.Storage;

public interface IStorageService
{
    // Saves a file stream and returns the storage key
    Task<string> SaveFileAsync(
        Stream content,
        string relativePath,
        CancellationToken cancellationToken = default
    );

    // Retrieves a file using its storage key
    Task<Stream?> GetFileAsync(string relativePath, CancellationToken cancellationToken = default);

    // Deletes a file using its storage key
    Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default);
}
