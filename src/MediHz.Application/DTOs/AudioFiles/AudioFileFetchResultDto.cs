using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Application.DTOs.AudioFiles;

public record AudioFileFetchResultDto(
    Guid audioId,
    Stream stream,
    string format,
    string contentType,
    string fileName

);