using FluentAssertions;
using MediHz.Application.DTOs.Annotations;
using MediHz.Domain.Annotations;
using MediHz.Domain.Audio;
using MediHz.Domain.Projects;
using MediHz.Domain.Users;
using MediHz.Infrastructure.Persistence;
using MediHz.Infrastructure.Persistence.Services.Annotations;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Services;

public class AnnotationServiceTests : UnitTestBase
{
    private readonly MediHzDbContext _dbContext = new(
        new DbContextOptionsBuilder<MediHzDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options
    );

    private readonly User _user;
    private readonly AudioFile _audioFile;
    private readonly LabelGroup _prePackagedGroup;
    private readonly Label _prePackagedLabel1;
    private readonly Label _prePackagedLabel2;
    private readonly AnnotationService _service;

    public AnnotationServiceTests(ITestOutputHelper output)
        : base(output)
    {
        _user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test Annotator",
            Email = "annotator@example.com",
            ProviderToken = "token-1",
        };

        var project = new Project("Heart Study", _user.Id);
        _audioFile = new AudioFile { FileName = "heart.wav", ProjectId = project.Id };

        _prePackagedGroup = new LabelGroup
        {
            Id = new Guid("00000000-0000-0000-0000-000000000001"),
            Name = "CirCor",
            OwnerId = null,
        };

        _prePackagedLabel1 = new Label
        {
            Id = new Guid("00000000-0000-0000-0000-000000000101"),
            Name = "cardiac.s1",
            LabelGroupId = _prePackagedGroup.Id,
            OwnerId = null,
        };

        _prePackagedLabel2 = new Label
        {
            Id = new Guid("00000000-0000-0000-0000-000000000102"),
            Name = "cardiac.systole",
            LabelGroupId = _prePackagedGroup.Id,
            OwnerId = null,
        };

        _dbContext.Users.Add(_user);
        _dbContext.Projects.Add(project);
        _dbContext.AudioFiles.Add(_audioFile);
        _dbContext.LabelGroups.Add(_prePackagedGroup);
        _dbContext.Labels.AddRange(_prePackagedLabel1, _prePackagedLabel2);
        _dbContext.SaveChanges();

        _service = new AnnotationService(_dbContext);
    }

    [Fact]
    public async Task CreateAsync_ShouldLinkExistingPrePackagedLabels_AndCreateUnassignedCustomLabels()
    {
        // Arrange
        var annotationId = Guid.NewGuid();
        var request = new CreateAnnotationRequest(
            annotationId,
            _audioFile.Id,
            ["cardiac.s1", "custom.observation", " cardiac.s1 "],
            1.0m,
            2.5m
        );

        // Act
        var result = await _service.CreateAsync(_user.Id, request);

        // Assert
        result.Id.Should().Be(annotationId);
        result.LabelNames.Should().BeEquivalentTo(["cardiac.s1", "custom.observation"]);

        var allLabels = await _dbContext.Labels.ToListAsync();
        allLabels.Should().HaveCount(3);

        var customLabel = allLabels.Single(l => l.Name == "custom.observation");
        customLabel.LabelGroupId.Should().BeNull();
        customLabel.OwnerId.Should().Be(_user.Id);

        var s1Label = allLabels.Single(l => l.Name == "cardiac.s1");
        s1Label.Id.Should().Be(_prePackagedLabel1.Id);
        s1Label.LabelGroupId.Should().Be(_prePackagedGroup.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReplaceLabelsWithPrePackagedAndCustomLabels()
    {
        // Arrange
        var annotationId = Guid.NewGuid();
        await _service.CreateAsync(
            _user.Id,
            new CreateAnnotationRequest(annotationId, _audioFile.Id, ["cardiac.s1"], 0.5m, 1.5m)
        );

        var updateRequest = new UpdateAnnotationRequest(
            ["cardiac.systole", "new.custom.tag"],
            0.5m,
            2.0m
        );

        // Act
        var updated = await _service.UpdateAsync(annotationId, updateRequest);

        // Assert
        updated.Should().NotBeNull();
        updated!.LabelNames.Should().BeEquivalentTo(["cardiac.systole", "new.custom.tag"]);

        var newCustom = await _dbContext.Labels.SingleAsync(l => l.Name == "new.custom.tag");
        newCustom.LabelGroupId.Should().BeNull();
        newCustom.OwnerId.Should().Be(_user.Id);
    }
}
