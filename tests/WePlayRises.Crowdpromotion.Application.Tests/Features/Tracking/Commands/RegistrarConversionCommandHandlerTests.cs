using System.Net;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Commands;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Mapping;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Tracking.Commands;

public class RegistrarConversionCommandHandlerTests
{
    private readonly Mock<IPromoEventoService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<RegistrarConversionCommand>> _validatorMock;
    private readonly Mock<ILogger<RegistrarConversionCommandHandler>> _loggerMock;
    private readonly RegistrarConversionCommandHandler _sut;

    public RegistrarConversionCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromoEventoService>();
        _validatorMock = new Mock<IValidator<RegistrarConversionCommand>>();
        _loggerMock = new Mock<ILogger<RegistrarConversionCommandHandler>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PromoEventoProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _sut = new RegistrarConversionCommandHandler(
            _serviceMock.Object,
            _mapper,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidConversion_ReturnsCreatedWithComision()
    {
        // Arrange
        var command = TrackingTestData.CreateValidConversionCommand();
        var conversionResult = TrackingTestData.CreateConversionResult(
            eventoId: TrackingTestData.DefaultEventoId,
            comision: 5.00m,
            acreditada: true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.RegistrarConversionAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversionResult);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.EventoId.Should().Be(conversionResult.EventoId);
        result.Data.ComisionCalculada.Should().Be(conversionResult.ComisionCalculada);
        result.Data.ComisionAcreditada.Should().BeTrue();
        result.Data.WalletTransaccionId.Should().Be(conversionResult.WalletTransaccionId);
        result.Data.MonedaNombre.Should().Be(conversionResult.MonedaNombre);
        result.Messages.Should().ContainSingle(m => m.HttpStatusCode == HttpStatusCode.Created);

        _serviceMock.Verify(
            s => s.RegistrarConversionAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = TrackingTestData.CreateValidConversionCommand();

        var validationFailures = new List<ValidationFailure>
        {
            new("CodigoReferido", "El codigo de referido es obligatorio")
            {
                ErrorCode = ServiceResponseMessageType.Validation_Required
            },
            new("ValorMonetario", "El valor monetario debe ser mayor a 0")
            {
                ErrorCode = ServiceResponseMessageType.Validation_ValorMonetarioInvalido
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().NotBeEmpty();
        result.Messages.Should().HaveCount(2);

        _serviceMock.Verify(
            s => s.RegistrarConversionAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrowsException_ReturnsInternalError()
    {
        // Arrange
        var command = TrackingTestData.CreateValidConversionCommand();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.RegistrarConversionAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_ConversionNotAcreditada_ReturnsNullMonedaNombre()
    {
        // Arrange
        var command = TrackingTestData.CreateValidConversionCommand();
        var conversionResult = TrackingTestData.CreateConversionResult(
            eventoId: TrackingTestData.DefaultEventoId,
            comision: 5.00m,
            acreditada: false);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.RegistrarConversionAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversionResult);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.ComisionAcreditada.Should().BeFalse();
        result.Data.MonedaNombre.Should().BeNull();
        result.Data.ComisionCalculada.Should().Be(5.00m);
    }

    [Fact]
    public async Task Handle_ValidConversion_SetsTipoEventoIdToBacking()
    {
        // Arrange
        var command = TrackingTestData.CreateValidConversionCommand();
        var conversionResult = TrackingTestData.CreateConversionResult();
        PromoEvento? capturedEntity = null;

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.RegistrarConversionAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()))
            .Callback<PromoEvento, CancellationToken>((entity, _) => capturedEntity = entity)
            .ReturnsAsync(conversionResult);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedEntity.Should().NotBeNull();
        capturedEntity!.TipoEventoId.Should().Be(4, "conversion events must have TipoEventoId = 4 (Backing)");
        capturedEntity.ImporteAsociado.Should().Be(command.ValorMonetario);
        capturedEntity.CodigoReferido.Should().Be(command.CodigoReferido);
        capturedEntity.MonedaId.Should().Be(command.MonedaId);
        capturedEntity.UserIdAfectado.Should().Be(command.UserIdAfectado);
        capturedEntity.FechaCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
