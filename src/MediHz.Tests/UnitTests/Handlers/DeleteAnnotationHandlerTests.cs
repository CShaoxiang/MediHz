using AutoFixture;
using FluentAssertions;
using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;
using MediHz.Application.Handlers.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class DeleteAnnotationHandlerTests : UnitTestBase
{
    private readonly Mock<IAnnotationService> _annotationServiceMock = new();

    private DeleteAnnotationHandler _handler;

    public DeleteAnnotationHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _handler = new(_annotationServiceMock.Object, Logger<DeleteAnnotationHandler>());
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenAnnotationIsDeletedSuccessfully()
    {
        // Arrange
        var request = Fixture.Create<DeleteAnnotationCommand>();

        _annotationServiceMock
            .Setup(s => s.DeleteAsync(request.AnnotationId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenAnnotationDoesNotExist()
    {
        // Arrange
        var request = Fixture.Create<DeleteAnnotationCommand>();

        _annotationServiceMock
            .Setup(s => s.DeleteAsync(request.AnnotationId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("Annotation Not Found");
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var request = Fixture.Create<DeleteAnnotationCommand>();
        var exceptionMessage = "Something went wrong";

        _annotationServiceMock
            .Setup(s => s.DeleteAsync(request.AnnotationId, CancellationToken.None))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("Annotation Deletion Failed");
        result.Problem.Detail.Should().Be(exceptionMessage);
    }
}
