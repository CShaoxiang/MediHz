using AutoFixture;
using FluentAssertions;
using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.DTOs.Annotations;
using MediHz.Application.Handlers.Annotations;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class CreateLabelHandlerTests : UnitTestBase
{
    private readonly Mock<ILabelService> _labelServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Guid _userId = Guid.NewGuid();

    private CreateLabelHandler _handler;

    public CreateLabelHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _currentUserMock.SetupGet(user => user.UserId).Returns(_userId);
        _handler = new(
            _labelServiceMock.Object,
            _currentUserMock.Object,
            Logger<CreateLabelHandler>()
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnCreated_WhenLabelIsCreated()
    {
        // Arrange
        var request = new CreateLabelRequest("New Label", Guid.NewGuid());
        var labelDto = Fixture.Create<LabelDto>();
        _labelServiceMock
            .Setup(s => s.CreateAsync(_userId, request, CancellationToken.None))
            .ReturnsAsync(labelDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().BeEquivalentTo(labelDto);
        _labelServiceMock.Verify(
            service => service.CreateAsync(_userId, request, CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldPassNullGroupId_WhenLabelIsUnassigned()
    {
        // Arrange
        var request = new CreateLabelRequest("Unassigned Label", null);
        var labelDto = Fixture.Build<LabelDto>().With(l => l.LabelGroupId, (Guid?)null).Create();
        _labelServiceMock
            .Setup(s => s.CreateAsync(_userId, request, CancellationToken.None))
            .ReturnsAsync(labelDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value!.LabelGroupId.Should().BeNull();
        _labelServiceMock.Verify(
            service => service.CreateAsync(_userId, request, CancellationToken.None),
            Times.Once
        );
    }
}
