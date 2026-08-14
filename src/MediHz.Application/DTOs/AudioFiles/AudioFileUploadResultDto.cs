namespace MediHz.Application.DTOs.AudioFiles;

public record BatchAudioFileUploadResultDto(
    Guid BatchId,
    IReadOnlyList<AudioFileUploadResultDto> Items
    );

public record AudioFileUploadResultDto(
    Guid Id,
    string FileName,
    string ?  MimeType,
    long SizeBytes,
    Status Status,
    string ? ErrorMessage
    );

public enum Status
{
    Success = 1,
    Failure = 0
}