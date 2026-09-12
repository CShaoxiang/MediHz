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

public class GetAllLabelGroupsHandlerTests : UnitTestBase
{
    private readonly Mock<ILabelGroupService> _labelGroupServiceMock = new();

    private GetAllLabelGroupsHandler _handler;

    public GetAllLabelGroupsHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _handler = new(_labelGroupServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnLabelGroups_WhenSomeExist()
    {
        // Arrange
        var request = new EmptyRequest();
        var labelGroups = Fixture.CreateMany<LabelGroupDto>(2).ToArray();
        _labelGroupServiceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(labelGroups);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(labelGroups);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyListSuccess_WhenNoneExist()
    {
        // Arrange
        var request = new EmptyRequest();
        _labelGroupServiceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEmpty();
    }
}
