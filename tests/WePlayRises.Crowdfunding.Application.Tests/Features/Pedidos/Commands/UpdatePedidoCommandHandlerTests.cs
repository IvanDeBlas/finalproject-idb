using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
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

public class UpdatePedidoCommandHandlerTests
{
    private readonly Mock<IPedidoService> _serviceMock;
    private readonly Mock<IValidator<UpdatePedidoCommand>> _validatorMock;
    private readonly Mock<ILogger<UpdatePedidoCommandHandler>> _loggerMock;
    private readonly UpdatePedidoCommandHandler _sut;

    public UpdatePedidoCommandHandlerTests()
    {
        _serviceMock = new Mock<IPedidoService>();
        _validatorMock = new Mock<IValidator<UpdatePedidoCommand>>();
        _loggerMock = new Mock<ILogger<UpdatePedidoCommandHandler>>();
        _sut = new UpdatePedidoCommandHandler(
            _serviceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessAndUpdatesEntity()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var command = PedidoTestData.CreateValidUpdateCommand(pedidoId: pedidoId);
        var entity = PedidoTestData.CreateValid(pedidoId: pedidoId);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

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
        entity.EstadoPedidoId.Should().Be(2);
        entity.ImportePropina.Should().Be(10m);
        entity.ComentarioBacker.Should().Be("Updated comment");
        entity.FechaActualizacion.Should().NotBeNull();
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<PedidoCrowdfunding>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = new UpdatePedidoCommand { Id = Guid.Empty };

        var validationFailures = new List<ValidationFailure>
        {
            new("Id", "El Id es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<PedidoCrowdfunding>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PedidoNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = PedidoTestData.CreateValidUpdateCommand();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

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
    public async Task Handle_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var command = new UpdatePedidoCommand
        {
            Id = pedidoId,
            ComentarioBacker = "New comment only"
        };

        var entity = PedidoTestData.CreateValid(pedidoId: pedidoId);
        var originalEstado = entity.EstadoPedidoId;
        var originalPropina = entity.ImportePropina;

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

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
        entity.ComentarioBacker.Should().Be("New comment only");
        entity.EstadoPedidoId.Should().Be(originalEstado);
        entity.ImportePropina.Should().Be(originalPropina);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = PedidoTestData.CreateValidUpdateCommand();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

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
