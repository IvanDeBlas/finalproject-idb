using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Commands;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Wallet.Commands;

public class SolicitarCobroCommandHandlerTests
{
    private readonly Mock<IPromotorWalletService> _serviceMock;
    private readonly Mock<IValidator<SolicitarCobroCommand>> _validatorMock;
    private readonly Mock<ILogger<SolicitarCobroCommandHandler>> _loggerMock;
    private readonly SolicitarCobroCommandHandler _sut;

    public SolicitarCobroCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromotorWalletService>();
        _validatorMock = new Mock<IValidator<SolicitarCobroCommand>>();
        _loggerMock = new Mock<ILogger<SolicitarCobroCommandHandler>>();

        _sut = new SolicitarCobroCommandHandler(
            _serviceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCobro_ReturnsCreatedResponse()
    {
        // Arrange
        var command = WalletTestData.CreateSolicitarCobroCommand(importe: 25.00m);
        var promotor = WalletTestData.CreatePromotor();
        var cobroResult = WalletTestData.CreateCobroResult(importe: 25.00m, saldoRestante: 75.00m);

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<SolicitarCobroCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock
            .Setup(s => s.SolicitarCobroAsync(
                It.IsAny<PromotorId>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(SolicitarCobroResult?, CobroError)>((cobroResult, CobroError.None)));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TransaccionId.Should().Be(cobroResult.TransaccionId);
        result.Data.Importe.Should().Be(25.00m);
        result.Data.MonedaNombre.Should().Be("EUR");
        result.Data.EstadoTransaccionNombre.Should().Be("Pendiente");
        result.Data.SaldoRestante.Should().Be(75.00m);
        result.Messages.Should().ContainSingle(m =>
            m.HttpStatusCode == System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = new SolicitarCobroCommand { UserId = string.Empty, Importe = -1m };
        var failures = new List<ValidationFailure>
        {
            new("Importe", "El importe debe ser mayor que cero")
            {
                ErrorCode = ServiceResponseMessageType.Validation_RangeOutOfBounds
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Validation_RangeOutOfBounds);
        _serviceMock.Verify(
            s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_PromotorNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = WalletTestData.CreateSolicitarCobroCommand();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Promotor?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Promotor);
    }

    [Fact]
    public async Task Handle_WalletNoEncontrado_ReturnsNotFound()
    {
        // Arrange
        var command = WalletTestData.CreateSolicitarCobroCommand();
        var promotor = WalletTestData.CreatePromotor();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock
            .Setup(s => s.SolicitarCobroAsync(
                It.IsAny<PromotorId>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(SolicitarCobroResult?, CobroError)>((null, CobroError.WalletNoEncontrado)));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Wallet);
    }

    [Fact]
    public async Task Handle_SaldoInsuficiente_ReturnsConflict()
    {
        // Arrange
        var command = WalletTestData.CreateSolicitarCobroCommand(importe: 999.00m);
        var promotor = WalletTestData.CreatePromotor();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock
            .Setup(s => s.SolicitarCobroAsync(
                It.IsAny<PromotorId>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(SolicitarCobroResult?, CobroError)>((null, CobroError.SaldoInsuficiente)));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_SaldoInsuficiente);
    }

    [Fact]
    public async Task Handle_SaldoBajoMinimo_ReturnsConflict()
    {
        // Arrange
        var command = WalletTestData.CreateSolicitarCobroCommand(importe: 5.00m);
        var promotor = WalletTestData.CreatePromotor();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock
            .Setup(s => s.SolicitarCobroAsync(
                It.IsAny<PromotorId>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(SolicitarCobroResult?, CobroError)>((null, CobroError.SaldoBajoMinimo)));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_SaldoBajoMinimoRetiro);
    }

    [Fact]
    public async Task Handle_CobroConcurrente_ReturnsConflict()
    {
        // Arrange
        var command = WalletTestData.CreateSolicitarCobroCommand();
        var promotor = WalletTestData.CreatePromotor();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock
            .Setup(s => s.SolicitarCobroAsync(
                It.IsAny<PromotorId>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(SolicitarCobroResult?, CobroError)>((null, CobroError.CobroConcurrente)));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_CobroConcurrente);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var command = WalletTestData.CreateSolicitarCobroCommand();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB connection failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
