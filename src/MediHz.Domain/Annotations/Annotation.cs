using MediHz.Domain.Audio;
using MediHz.Domain.Core;
using MediHz.Domain.Users;

namespace MediHz.Domain.Annotations;

public class Annotation : EntityBase
{
    public Guid AudioFileId { get; set; }
    public AudioFile AudioFile { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public decimal StartTimeSeconds { get; set; }
    public decimal EndTimeSeconds { get; set; }
    public decimal DurationSeconds { get; set; }
}