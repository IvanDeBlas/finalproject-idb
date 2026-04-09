using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Campanias.Commands;

public class DeleteCampaniaCommandHandlerTests
{
    private readonly Mock<ICampaniaService> _serviceMock;
    private readonly Mock<ILogger<DeleteCampaniaCommandHandler>> _loggerMock;
    private readonly DeleteCampaniaCommandHandler _sut;

    public DeleteCampaniaCommandHandlerTests()
    {
        _serviceMock = new Mock<ICampaniaService>();
        _loggerMock = new Mock<ILogger<DeleteCampaniaCommandHandler>>();
        _sut = new DeleteCampaniaCommandHandler(
            _serviceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingCampania_ReturnsTrueAndSoftDeletes()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var command = new DeleteCampaniaCommand(campaniaId);
        var entity = CampaniaTestData.CreateValidBorrador(campaniaId: campaniaId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        entity.Borrado.Should().BeTrue();
        entity.FechaActualizacion.Should().NotBeNull();
        _serviceMock.Verify(s => s.UpdateAsync(It.Is<CampaniaCrowdfunding>(c => c.Borrado), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentCampania_ReturnsNotFound()
    {
        // Arrange
        var command = new DeleteCampaniaCommand(Guid.NewGuid());

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CampaniaCrowdfunding?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Campania);
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = new DeleteCampaniaCommand(Guid.NewGuid());

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
