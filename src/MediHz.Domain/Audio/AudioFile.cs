using MediHz.Domain.Core;
using MediHz.Domain.Projects;
using MediHz.Domain.Users;

namespace MediHz.Domain.Audio;

public class AudioFile : EntityBase
{
    public string FileName { get; set; } = null!;
    public AudioFileFormat Format { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public Guid? UploaderId { get; set; }
    public User? Uploader { get; set; }
    public decimal Duration { get; set; }
    public decimal SampleRate { get; set; }
    public int Channels { get; set; }
    public long SizeBytes { get; set; }
}
public enum AudioFileFormat
{
    Mp3 = 0,
    Wav = 1
}
