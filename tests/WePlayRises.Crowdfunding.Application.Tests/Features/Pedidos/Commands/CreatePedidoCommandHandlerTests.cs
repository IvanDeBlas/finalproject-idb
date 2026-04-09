using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Pedidos.Commands;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Pedidos.Commands;

public class CreatePedidoCommandHandlerTests
{
    private readonly Mock<IPedidoService> _serviceMock;
    private readonly Mock<ICampaniaService> _campaniaServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreatePedidoCommand>> _validatorMock;
    private readonly Mock<ILogger<CreatePedidoCommandHandler>> _loggerMock;
    private readonly CreatePedidoCommandHandler _sut;

    public CreatePedidoCommandHandlerTests()
    {
        _serviceMock = new Mock<IPedidoService>();
        _campaniaServiceMock = new Mock<ICampaniaService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreatePedidoCommand>>();
        _loggerMock = new Mock<ILogger<CreatePedidoCommandHandler>>();
        _sut = new CreatePedidoCommandHandler(
            _serviceMock.Object,
            _campaniaServiceMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithPedidoDto()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var pedidoId = Guid.NewGuid();
        var command = PedidoTestData.CreateValidCommand(campaniaId: campaniaId);
        var entity = PedidoTestData.CreateValid(pedidoId: pedidoId, campaniaId: campaniaId);
        var campania = CampaniaTestData.CreatePublicada(campaniaId: campaniaId);
        var expectedDto = new PedidoDto
        {
            Id = pedidoId,
            CampaniaId = campaniaId,
            ImporteTotal = command.ImporteTotal
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _mapperMock
            .Setup(m => m.Map<PedidoCrowdfunding>(command))
            .Returns(entity);

        var returnedId = new PedidoCrowdfundingId(pedidoId);
        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<PedidoCrowdfunding>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnedId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(returnedId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(m => m.Map<PedidoDto>(entity))
            .Returns(expectedDto);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(pedidoId);
        result.Data.CampaniaId.Should().Be(campaniaId);
        _serviceMock.Verify(s => s.CreateAsync(It.IsAny<PedidoCrowdfunding>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = new CreatePedidoCommand
        {
            CampaniaId = Guid.Empty,
            MonedaId = 0,
            ImporteTotal = 0
        };

        var validationFailures = new List<ValidationFailure>
        {
            new("CampaniaId", "El CampaniaId es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
            new("ImporteTotal", "El importe total debe ser mayor a 0") { ErrorCode = ServiceResponseMessageType.Validation_InvalidAmount }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().HaveCount(2);
        _serviceMock.Verify(s => s.CreateAsync(It.IsAny<PedidoCrowdfunding>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CampaniaNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = PedidoTestData.CreateValidCommand();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CampaniaCrowdfunding?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Campania);
        _serviceMock.Verify(s => s.CreateAsync(It.IsAny<PedidoCrowdfunding>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = PedidoTestData.CreateValidCommand();
        var campania = CampaniaTestData.CreatePublicada();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _mapperMock
            .Setup(m => m.Map<PedidoCrowdfunding>(command))
            .Returns(PedidoTestData.CreateValid());

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<PedidoCrowdfunding>(), It.IsAny<CancellationToken>()))
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
