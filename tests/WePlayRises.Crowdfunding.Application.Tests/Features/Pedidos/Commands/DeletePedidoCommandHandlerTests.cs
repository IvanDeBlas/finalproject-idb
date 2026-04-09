using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Features.Pedidos.Commands;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Pedidos.Commands;

public class DeletePedidoCommandHandlerTests
{
    private readonly Mock<IPedidoService> _serviceMock;
    private readonly Mock<ILogger<DeletePedidoCommandHandler>> _loggerMock;
    private readonly DeletePedidoCommandHandler _sut;

    public DeletePedidoCommandHandlerTests()
    {
        _serviceMock = new Mock<IPedidoService>();
        _loggerMock = new Mock<ILogger<DeletePedidoCommandHandler>>();
        _sut = new DeletePedidoCommandHandler(
            _serviceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingPedido_CancelsAndReturnsTrue()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var command = new DeletePedidoCommand(pedidoId);
        var entity = PedidoTestData.CreateValid(pedidoId: pedidoId, estadoPedidoId: 1);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PedidoCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<PedidoCrowdfunding>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        entity.EstadoPedidoId.Should().Be(5); // Cancelado
        entity.FechaActualizacion.Should().NotBeNull();
        _serviceMock.Verify(s => s.UpdateAsync(It.Is<PedidoCrowdfunding>(p => p.EstadoPedidoId == 5), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentPedido_ReturnsNotFound()
    {
        // Arrange
        var command = new DeletePedidoCommand(Guid.NewGuid());

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PedidoCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PedidoCrowdfunding?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Backing);
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<PedidoCrowdfunding>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = new DeletePedidoCommand(Guid.NewGuid());

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PedidoCrowdfundingId>(), It.IsAny<CancellationToken>()))
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
