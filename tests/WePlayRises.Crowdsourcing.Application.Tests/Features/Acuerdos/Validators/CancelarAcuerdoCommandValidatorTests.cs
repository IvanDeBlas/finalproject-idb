using FluentAssertions;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Validators;

public class CancelarAcuerdoCommandValidatorTests
{
    private readonly CancelarAcuerdoCommandValidator _sut;

    public CancelarAcuerdoCommandValidatorTests()
    {
        _sut = new CancelarAcuerdoCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        // Arrange
        var command = new CancelarAcuerdoCommand
        {
            AcuerdoId = Guid.NewGuid(),
            UserId = "user-1",
            Motivo = "No puedo continuar con este proyecto por motivos personales"
        };

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyAcuerdoId_Fails()
    {
        // Arrange
        var command = new CancelarAcuerdoCommand
        {
            AcuerdoId = Guid.Empty,
            Motivo = "No puedo continuar con este proyecto por motivos personales"
        };

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyMotivo_Fails()
    {
        // Arrange
        var command = new CancelarAcuerdoCommand
        {
            AcuerdoId = Guid.NewGuid(),
            Motivo = ""
        };

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_MotivoTooShort_Fails()
    {
        // Arrange
        var command = new CancelarAcuerdoCommand
        {
            AcuerdoId = Guid.NewGuid(),
            Motivo = "Corto"
        };

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_MinLength);
    }

    [Fact]
    public async Task Validate_MotivoTooLong_Fails()
    {
        // Arrange
        var command = new CancelarAcuerdoCommand
        {
            AcuerdoId = Guid.NewGuid(),
            Motivo = new string('a', 1001)
        };

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_MaxLength);
    }
}
