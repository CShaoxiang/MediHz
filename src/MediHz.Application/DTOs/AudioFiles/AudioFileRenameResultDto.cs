using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Application.DTOs.AudioFiles;

public record AudioFileRenameResultDto(
    Guid audioId,
    string audioName

);