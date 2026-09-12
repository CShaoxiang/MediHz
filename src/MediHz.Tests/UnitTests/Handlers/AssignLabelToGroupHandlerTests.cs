using AutoFixture;
using FluentAssertions;
using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;
using MediHz.Application.Handlers.Annotations;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class AssignLabelToGroupHandlerTests : UnitTestBase
{
    private readonly Mock<ILabelService> _labelServiceMock = new();

    private AssignLabelToGroupHandler _handler;

    public AssignLabelToGroupHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _handler = new(_labelServiceMock.Object, Logger<AssignLabelToGroupHandler>());
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenLabelIsReassigned()
    {
        // Arrange
        var request = new AssignLabelRequest(Guid.NewGuid(), Guid.NewGuid());
        var labelDto = Fixture.Create<LabelDto>();
        _labelServiceMock
            .Setup(s =>
                s.AssignToGroupAsync(request.LabelId, request.TargetGroupId, CancellationToken.None)
            )
            .ReturnsAsync(labelDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(labelDto);
    }

    [Fact]
    public async Task Handle_ShouldAllowNullTargetGroupId_WhenUnassigningLabel()
    {
        // Arrange
        var request = new AssignLabelRequest(Guid.NewGuid(), null);
        var labelDto = Fixture.Build<LabelDto>().With(l => l.LabelGroupId, (Guid?)null).Create();
        _labelServiceMock
            .Setup(s => s.AssignToGroupAsync(request.LabelId, null, CancellationToken.None))
            .ReturnsAsync(labelDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value!.LabelGroupId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenLabelOrGroupDoesNotExist()
    {
        // Arrange
        var request = new AssignLabelRequest(Guid.NewGuid(), Guid.NewGuid());
        _labelServiceMock
            .Setup(s =>
                s.AssignToGroupAsync(request.LabelId, request.TargetGroupId, CancellationToken.None)
            )
            .ReturnsAsync((LabelDto?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("Not Found");
    }
}
