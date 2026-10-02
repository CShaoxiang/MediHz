using System.Text;
using FluentAssertions;
using MediHz.Application.Abstractions.Storage;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Domain.Audio;
using MediHz.Domain.Projects;
using MediHz.Infrastructure.Persistence;
using MediHz.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Services;

public class AudioFileServiceTests : UnitTestBase
{
    private readonly MediHzDbContext _dbContext = new(
        new DbContextOptionsBuilder<MediHzDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options
    );

    public readonly IStorageService _storageService = new LocalStorageService();

    private AudioFileUploadService _audioUploadService;

    private AudioFileDeleteService _deleteService;        
    private AudioFileFetchService _fetchService;

    private Project _project;

    public AudioFileServiceTests(ITestOutputHelper output) : base(output)
    {
        _project = new Project("Sample Project Name", Guid.NewGuid());
        _dbContext.Projects.Add(_project);
        _dbContext.SaveChanges();

        _audioUploadService = new (_storageService,_dbContext, Logger<AudioFileUploadService>());
        _deleteService = new (_dbContext, _storageService);
        _fetchService = new (_storageService, Logger<AudioFileFetchService>());
    }

    Guid testGuid = Guid.NewGuid();

    [Fact]
    public async Task RenameAudioFile_ShouldUpdateFileName_WhenFileExists()
    {
        // Arrange
        var audioFile = new AudioFile
        {
            FileName = "original.mp3",
            Project = _project,
            ProjectId = _project.Id,
            Format = AudioFileFormat.Mp3,
            Duration = 120,
            SampleRate = 44100,
            Channels = 2,
            SizeBytes = 1024,
        };

        _dbContext.AudioFiles.Add(audioFile);
        await _dbContext.SaveChangesAsync();

        var service = new AudioFilePatchService(_dbContext);
        var changes = new AudioFilePatchDto(FileName: "renamed.mp3");

        // Act
        var result = await service.patchAudioFileAsync(_project.Id, audioFile.Id, changes, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(audioFile.Id);
        result.FileName.Should().Be("renamed.mp3");

        var updatedAudioFile = await _dbContext.AudioFiles.FindAsync([audioFile.Id],CancellationToken.None);
        updatedAudioFile.Should().NotBeNull();
        updatedAudioFile.FileName.Should().Be("renamed.mp3");
    }

    [Fact]
    public async Task DeleteAudioFile_ShouldRemoveAudio_WhenFileExists()
    {
        // Arrange
        using var wavStream = CreateDummyWavStream(44100, 10); // 10 seconds is valid

        var file = new FormFile(wavStream, 0, wavStream.Length, "file", "valid.wav")
        {
            Headers = new HeaderDictionary(),
            ContentType = "audio/wav",
        };

        // Act
        var result = await _audioUploadService.UploadBatchAsync(_project.Id, [file]);

        var audio = result.Items[0];

        var deleteService = new AudioFileDeleteService(_dbContext, _storageService);        
        var fetchService = new AudioFileFetchService(_storageService, Logger<AudioFileFetchService>());

        var storedFile = await fetchService.getAudioFileAsync(
            _project.Id,
            audio.Id,
            audio.MimeType,
            CancellationToken.None);
        storedFile.Should().NotBeNull();
        storedFile.stream.Length.Should().Be(wavStream.Length);
        await storedFile.stream.DisposeAsync();

        var deletion = await deleteService.deleteAudioFileAsync(_project.Id,audio.Id,audio.MimeType, CancellationToken.None);

        // Assert
        deletion.Should().NotBeNull();
        deletion.status.Should().Be(Status.Success);

        var deletedAudio = await _dbContext.AudioFiles.FindAsync(audio.Id);
        deletedAudio.Should().BeNull();

        await FluentActions.Invoking(() => fetchService.getAudioFileAsync(
                _project.Id,
                audio.Id,
                audio.MimeType,
                CancellationToken.None))
            .Should()
            .ThrowAsync<FileNotFoundException>();
    }


   private static MemoryStream CreateDummyWavStream(int sampleRate, int durationSeconds)
    {
        var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms, Encoding.UTF8, true);

        short channels = 1;
        short bitsPerSample = 16;
        int subChunk2Size = sampleRate * channels * (bitsPerSample / 8) * durationSeconds;
        int chunkSize = 36 + subChunk2Size;

        writer.Write(Encoding.UTF8.GetBytes("RIFF"));
        writer.Write(chunkSize);
        writer.Write(Encoding.UTF8.GetBytes("WAVE"));
        writer.Write(Encoding.UTF8.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);
        writer.Write(channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * (bitsPerSample / 8));
        writer.Write((short)(channels * (bitsPerSample / 8)));
        writer.Write(bitsPerSample);
        writer.Write(Encoding.UTF8.GetBytes("data"));
        writer.Write(subChunk2Size);

        writer.Write(new byte[subChunk2Size]);

        ms.Position = 0;
        return ms;
    }
}