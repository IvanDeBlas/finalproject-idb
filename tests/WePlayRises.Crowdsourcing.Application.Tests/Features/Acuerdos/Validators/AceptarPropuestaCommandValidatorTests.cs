using FluentAssertions;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Validators;

public class AceptarPropuestaCommandValidatorTests
{
    private readonly Mock<IPropuestaCrowdsourcingService> _propuestaServiceMock;
    private readonly Mock<IAcuerdoCrowdsourcingService> _acuerdoServiceMock;
    private readonly AceptarPropuestaCommandValidator _sut;

    public AceptarPropuestaCommandValidatorTests()
    {
        _propuestaServiceMock = new Mock<IPropuestaCrowdsourcingService>();
        _acuerdoServiceMock = new Mock<IAcuerdoCrowdsourcingService>();
        _sut = new AceptarPropuestaCommandValidator(
            _propuestaServiceMock.Object,
            _acuerdoServiceMock.Object);
    }

    private AceptarPropuestaCommand CreateValidCommand()
    {
        return new AceptarPropuestaCommand
        {
            PropuestaId = Guid.NewGuid(),
            UserId = "user-1",
            TituloInterno = "Mi acuerdo de trabajo",
            FechaInicio = DateTime.UtcNow
        };
    }

    private void SetupValidAsyncRules(AceptarPropuestaCommand command)
    {
        var propuesta = CrowdsourcingTestData.CreatePropuestaPendiente();
        _propuestaServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propuesta);
        _acuerdoServiceMock.Setup(s => s.ExisteAcuerdoActivoParaNecesidadAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }

    [Fact]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidAsyncRules(command);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyTitulo_ReturnsValidationError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TituloInterno = "";
        SetupValidAsyncRules(command);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_TituloTooLong_ReturnsValidationError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TituloInterno = new string('a', 201);
        SetupValidAsyncRules(command);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_FechaFinBeforeFechaInicio_ReturnsValidationError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.FechaFinPrevista = command.FechaInicio.AddDays(-1);
        SetupValidAsyncRules(command);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_InvalidDate);
    }
}
