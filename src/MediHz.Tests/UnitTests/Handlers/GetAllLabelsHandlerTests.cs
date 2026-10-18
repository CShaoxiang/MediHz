using AutoFixture;
using FluentAssertions;
using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;
using MediHz.Application.Handlers;
using MediHz.Application.Handlers.Annotations;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class GetAllLabelsHandlerTests : UnitTestBase
{
    private readonly Mock<ILabelService> _labelServiceMock = new();

    private readonly GetAllLabelsHandler _handler;

    public GetAllLabelsHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _handler = new(_labelServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnLabels_WhenSomeExist()
    {
        // Arrange
        var request = new EmptyRequest();
        var labels = Fixture.CreateMany<LabelDto>(3).ToArray();
        _labelServiceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(labels);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(labels);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyListSuccess_WhenNoneExist()
    {
        // Arrange
        var request = new EmptyRequest();
        _labelServiceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEmpty();
    }
}
