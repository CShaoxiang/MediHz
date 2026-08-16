using FluentAssertions;
using MediHz.Application.Handlers;

namespace MediHz.Tests.UnitTests.Handlers;

public class SampleHandlerTests
{

    [Fact]
    public async Task Handle_ShouldReturnDefault_WhenNoNameProvided()
    {
        // Arrange
        var handler = new SampleHandler();
        var request = new SampleRequest();

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().Be("Hello, World!");
    }

    [Theory]
    [InlineData("Anthony")]
    [InlineData("Daniel")]
    [InlineData("Ozair")]
    [InlineData("Sam")]
    [InlineData("Simon")]
    public async Task Handle_ShouldReturnWithSpecifiedName_WhenNameProvided(string name)
    {
        // Arrange
        var handler = new SampleHandler();
        var request = new SampleRequest(name);

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().Be($"Hello, {name}!");

    }
}
