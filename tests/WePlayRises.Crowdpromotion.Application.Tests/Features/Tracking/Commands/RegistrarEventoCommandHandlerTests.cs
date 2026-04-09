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

public class RegistrarEventoCommandHandlerTests
{
    private readonly Mock<IPromoEventoService> _serviceMock;
    private readonly Mock<ITrackingRateLimitService> _rateLimitMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<RegistrarEventoCommand>> _validatorMock;
    private readonly Mock<ILogger<RegistrarEventoCommandHandler>> _loggerMock;
    private readonly RegistrarEventoCommandHandler _sut;

    public RegistrarEventoCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromoEventoService>();
        _rateLimitMock = new Mock<ITrackingRateLimitService>();
        _validatorMock = new Mock<IValidator<RegistrarEventoCommand>>();
        _loggerMock = new Mock<ILogger<RegistrarEventoCommandHandler>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PromoEventoProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _sut = new RegistrarEventoCommandHandler(
            _serviceMock.Object,
            _rateLimitMock.Object,
            _mapper,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidClickEvent_ReturnsCreatedWithEventoId()
    {
        // Arrange
        var command = TrackingTestData.CreateValidEventoCommand(tipoEvento: 1);
        var expectedEventoId = TrackingTestData.DefaultEventoId;

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _rateLimitMock
            .Setup(r => r.IsRateLimitedAsync(command.IpOrigen!, command.CodigoReferido, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _serviceMock
            .Setup(s => s.RegistrarEventoAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEventoId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.EventoId.Should().Be(expectedEventoId);
        result.Data.Registrado.Should().BeTrue();
        result.Messages.Should().ContainSingle(m => m.HttpStatusCode == HttpStatusCode.Created);

        _serviceMock.Verify(
            s => s.RegistrarEventoAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _rateLimitMock.Verify(
            r => r.RegisterClickAsync(command.IpOrigen!, command.CodigoReferido, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = TrackingTestData.CreateValidEventoCommand();

        var validationFailures = new List<ValidationFailure>
        {
            new("CodigoReferido", "El codigo de referido es obligatorio")
            {
                ErrorCode = ServiceResponseMessageType.Validation_Required
            },
            new("TipoEventoPromoId", "El tipo de evento es invalido")
            {
                ErrorCode = ServiceResponseMessageType.Validation_TipoEventoInvalido
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

        _rateLimitMock.Verify(
            r => r.IsRateLimitedAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _serviceMock.Verify(
            s => s.RegistrarEventoAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ClickRateLimited_ReturnsRateLimitError()
    {
        // Arrange
        var command = TrackingTestData.CreateValidEventoCommand(tipoEvento: 1);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _rateLimitMock
            .Setup(r => r.IsRateLimitedAsync(command.IpOrigen!, command.CodigoReferido, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_RateLimitExcedido);

        _serviceMock.Verify(
            s => s.RegistrarEventoAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _rateLimitMock.Verify(
            r => r.RegisterClickAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_PageViewEvent_SkipsRateLimitCheck()
    {
        // Arrange
        var command = TrackingTestData.CreateValidEventoCommand(tipoEvento: 2);
        var expectedEventoId = TrackingTestData.DefaultEventoId;

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.RegistrarEventoAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEventoId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.EventoId.Should().Be(expectedEventoId);
        result.Data.Registrado.Should().BeTrue();

        _rateLimitMock.Verify(
            r => r.IsRateLimitedAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _rateLimitMock.Verify(
            r => r.RegisterClickAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _serviceMock.Verify(
            s => s.RegistrarEventoAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceThrowsException_ReturnsInternalError()
    {
        // Arrange
        var command = TrackingTestData.CreateValidEventoCommand(tipoEvento: 1);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _rateLimitMock
            .Setup(r => r.IsRateLimitedAsync(command.IpOrigen!, command.CodigoReferido, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _serviceMock
            .Setup(s => s.RegistrarEventoAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);

        _rateLimitMock.Verify(
            r => r.RegisterClickAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ClickWithNullIpOrigen_SkipsRateLimitCheck()
    {
        // Arrange
        var command = TrackingTestData.CreateValidEventoCommand(tipoEvento: 1);
        command.IpOrigen = null;
        var expectedEventoId = TrackingTestData.DefaultEventoId;

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.RegistrarEventoAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEventoId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.EventoId.Should().Be(expectedEventoId);

        _rateLimitMock.Verify(
            r => r.IsRateLimitedAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _rateLimitMock.Verify(
            r => r.RegisterClickAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ValidClickEvent_MapsCommandToPromoEventoCorrectly()
    {
        // Arrange
        var command = TrackingTestData.CreateValidEventoCommand(tipoEvento: 1);
        var expectedEventoId = TrackingTestData.DefaultEventoId;
        PromoEvento? capturedEntity = null;

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _rateLimitMock
            .Setup(r => r.IsRateLimitedAsync(command.IpOrigen!, command.CodigoReferido, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _serviceMock
            .Setup(s => s.RegistrarEventoAsync(It.IsAny<PromoEvento>(), It.IsAny<CancellationToken>()))
            .Callback<PromoEvento, CancellationToken>((entity, _) => capturedEntity = entity)
            .ReturnsAsync(expectedEventoId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedEntity.Should().NotBeNull();
        capturedEntity!.TipoEventoId.Should().Be(command.TipoEventoPromoId);
        capturedEntity.CodigoReferido.Should().Be(command.CodigoReferido);
        capturedEntity.IpOrigen.Should().Be(command.IpOrigen);
        capturedEntity.UrlOrigen.Should().Be(command.UrlOrigen);
        capturedEntity.UrlReferer.Should().Be(command.UrlReferer);
        capturedEntity.UtmSource.Should().Be(command.UtmSource);
        capturedEntity.UtmMedium.Should().Be(command.UtmMedium);
        capturedEntity.UtmCampaign.Should().Be(command.UtmCampaign);
        capturedEntity.UserIdAfectado.Should().Be(command.UserIdAfectado);
        capturedEntity.FechaCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
