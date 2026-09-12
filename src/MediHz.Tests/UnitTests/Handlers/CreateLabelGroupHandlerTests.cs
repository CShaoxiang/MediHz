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

public class CreateLabelGroupHandlerTests : UnitTestBase
{
    private readonly Mock<ILabelGroupService> _labelGroupServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private LabelGroupDto _labelGroupDto;

    private CreateLabelGroupHandler _handler;

    public CreateLabelGroupHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _labelGroupDto = Fixture.Create<LabelGroupDto>();
        _labelGroupServiceMock
            .Setup(s =>
                s.CreateAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CreateLabelGroupRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(_labelGroupDto);
        _currentUserMock.SetupGet(user => user.UserId).Returns(_labelGroupDto.OwnerId!.Value);

        _handler = new(
            _labelGroupServiceMock.Object,
            _currentUserMock.Object,
            Logger<CreateLabelGroupHandler>()
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnCreated_WhenLabelGroupIsCreated()
    {
        // Arrange
        var request = new CreateLabelGroupRequest("New Group");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().BeEquivalentTo(_labelGroupDto);
        _labelGroupServiceMock.Verify(
            service =>
                service.CreateAsync(_labelGroupDto.OwnerId!.Value, request, CancellationToken.None),
            Times.Once
        );
    }
}
