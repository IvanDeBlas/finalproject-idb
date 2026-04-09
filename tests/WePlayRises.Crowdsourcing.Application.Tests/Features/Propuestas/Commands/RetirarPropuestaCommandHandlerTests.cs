using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Propuestas.Commands;

public class RetirarPropuestaCommandHandlerTests
{
    private readonly Mock<IPropuestaCrowdsourcingService> _serviceMock;
    private readonly Mock<IValidator<RetirarPropuestaCommand>> _validatorMock;
    private readonly Mock<ILogger<RetirarPropuestaCommandHandler>> _loggerMock;
    private readonly RetirarPropuestaCommandHandler _sut;

    public RetirarPropuestaCommandHandlerTests()
    {
        _serviceMock = new Mock<IPropuestaCrowdsourcingService>();
        _validatorMock = new Mock<IValidator<RetirarPropuestaCommand>>();
        _loggerMock = new Mock<ILogger<RetirarPropuestaCommandHandler>>();
        _sut = new RetirarPropuestaCommandHandler(
            _serviceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<RetirarPropuestaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidCommand_RetiresPropuestaSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var necesidadId = new NecesidadCrowdsourcingId(Guid.NewGuid());
        var propuesta = CrowdsourcingTestData.CreatePropuesta(necesidadId);
        propuesta.UserId = userId;

        var command = new RetirarPropuestaCommand { Id = propuesta.Id.Value, UserId = userId };

        SetupValidValidation();

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propuesta);

        _serviceMock
            .Setup(s => s.RetirarAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(command.Id);
        result.Data.EstadoPropuestaNombre.Should().Be("Retirada");
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new("Id", "El ID de la propuesta es obligatorio")
            {
                ErrorCode = ServiceResponseMessageType.Validation_Required
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<RetirarPropuestaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var command = new RetirarPropuestaCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _serviceMock.Verify(s =>
            s.RetirarAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PropuestaNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = new RetirarPropuestaCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid().ToString()
        };

        SetupValidValidation();

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropuestaCrowdsourcing?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Propuesta);
    }

    [Fact]
    public async Task Handle_WrongOwner_ReturnsForbidden()
    {
        // Arrange
        var necesidadId = new NecesidadCrowdsourcingId(Guid.NewGuid());
        var propuesta = CrowdsourcingTestData.CreatePropuesta(necesidadId);
        propuesta.UserId = Guid.NewGuid().ToString();

        var command = new RetirarPropuestaCommand
        {
            Id = propuesta.Id.Value,
            UserId = Guid.NewGuid().ToString()
        };

        SetupValidValidation();

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propuesta);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var necesidadId = new NecesidadCrowdsourcingId(Guid.NewGuid());
        var propuesta = CrowdsourcingTestData.CreatePropuesta(necesidadId);
        propuesta.UserId = userId;

        var command = new RetirarPropuestaCommand { Id = propuesta.Id.Value, UserId = userId };

        SetupValidValidation();

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propuesta);

        _serviceMock
            .Setup(s => s.RetirarAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
