using FluentAssertions;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Validators;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Propuestas.Validators;

public class RetirarPropuestaCommandValidatorTests
{
    private readonly Mock<IPropuestaCrowdsourcingService> _propuestaServiceMock;
    private readonly RetirarPropuestaCommandValidator _sut;

    public RetirarPropuestaCommandValidatorTests()
    {
        _propuestaServiceMock = new Mock<IPropuestaCrowdsourcingService>();
        _sut = new RetirarPropuestaCommandValidator(_propuestaServiceMock.Object);
    }

    [Fact]
    public async Task Validate_ValidCommand_ReturnsValid()
    {
        // Arrange
        var necesidadId = new NecesidadCrowdsourcingId(Guid.NewGuid());
        var propuesta = CrowdsourcingTestData.CreatePropuesta(necesidadId, estadoPropuestaId: EstadoPropuestaConstants.Pendiente);
        var command = new RetirarPropuestaCommand { Id = propuesta.Id.Value, UserId = propuesta.UserId };

        _propuestaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propuesta);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyId_ReturnsError()
    {
        // Arrange
        var command = new RetirarPropuestaCommand { Id = Guid.Empty, UserId = Guid.NewGuid().ToString() };

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_PropuestaNotFound_ReturnsError()
    {
        // Arrange
        var command = new RetirarPropuestaCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid().ToString()
        };

        _propuestaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropuestaCrowdsourcing?)null);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.NotFound_Propuesta);
    }

    [Fact]
    public async Task Validate_PropuestaNotPendiente_ReturnsError()
    {
        // Arrange
        var necesidadId = new NecesidadCrowdsourcingId(Guid.NewGuid());
        var propuesta = CrowdsourcingTestData.CreatePropuesta(necesidadId, estadoPropuestaId: EstadoPropuestaConstants.Aceptada);
        var command = new RetirarPropuestaCommand { Id = propuesta.Id.Value, UserId = propuesta.UserId };

        _propuestaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propuesta);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.BusinessRule_PropuestaNotRetirable);
    }
}
