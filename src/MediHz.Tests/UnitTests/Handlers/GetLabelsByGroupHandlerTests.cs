using AutoFixture;
using FluentAssertions;
using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;
using MediHz.Application.Handlers.Annotations;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class GetLabelsByGroupHandlerTests : UnitTestBase
{
    private readonly Mock<ILabelService> _labelServiceMock = new();

    private GetLabelsByGroupHandler _handler;

    public GetLabelsByGroupHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _handler = new(_labelServiceMock.Object, Logger<GetLabelsByGroupHandler>());
    }

    [Fact]
    public async Task Handle_ShouldReturnLabels_WhenGroupIdIsProvided()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var labels = Fixture.CreateMany<LabelDto>(2).ToArray();
        _labelServiceMock
            .Setup(s => s.GetByGroupAsync(groupId, CancellationToken.None))
            .ReturnsAsync(labels);

        // Act
        var result = await _handler.Handle(groupId, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(labels);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnassignedLabels_WhenGroupIdIsNull()
    {
        // Arrange
        var labels = Fixture.CreateMany<LabelDto>(2).ToArray();
        _labelServiceMock
            .Setup(s => s.GetByGroupAsync(null, CancellationToken.None))
            .ReturnsAsync(labels);

        // Act
        var result = await _handler.Handle(null, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(labels);
        _labelServiceMock.Verify(s => s.GetByGroupAsync(null, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyListSuccess_WhenNoLabelsExist()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        _labelServiceMock
            .Setup(s => s.GetByGroupAsync(groupId, CancellationToken.None))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(groupId, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEmpty();
    }
}
