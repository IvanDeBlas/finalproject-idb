using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Commands;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Validators;
using WePlayRises.Crowdpromotion.Domain.Constants;
using Xunit;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Tracking.Validators;

public class RegistrarConversionCommandValidatorTests
{
    private readonly RegistrarConversionCommandValidator _sut;

    public RegistrarConversionCommandValidatorTests()
    {
        _sut = new RegistrarConversionCommandValidator();
    }

    private static RegistrarConversionCommand CreateValidCommand()
    {
        return new RegistrarConversionCommand
        {
            CodigoReferido = "REF-ABC123",
            CampaniaCrowdfundingId = Guid.NewGuid(),
            AportacionCrowdfundingId = Guid.NewGuid(),
            ValorMonetario = 50.00m,
            MonedaId = 1,
            UserIdAfectado = Guid.NewGuid().ToString()
        };
    }

    [Fact]
    public async Task Validate_ValidCommand_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyCodigoReferido_HasRequiredError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CodigoReferido = string.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CodigoReferido)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_CodigoReferidoTooLong_HasMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CodigoReferido = new string('X', 51);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CodigoReferido)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_EmptyCampaniaCrowdfundingId_HasRequiredError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CampaniaCrowdfundingId = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CampaniaCrowdfundingId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyAportacionCrowdfundingId_HasRequiredError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.AportacionCrowdfundingId = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AportacionCrowdfundingId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_ValorMonetarioZero_HasInvalidError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ValorMonetario = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ValorMonetario)
            .WithErrorCode(ServiceResponseMessageType.Validation_ValorMonetarioInvalido);
    }

    [Fact]
    public async Task Validate_NegativeValorMonetario_HasInvalidError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ValorMonetario = -10.50m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ValorMonetario)
            .WithErrorCode(ServiceResponseMessageType.Validation_ValorMonetarioInvalido);
    }

    [Fact]
    public async Task Validate_MonedaIdZero_HasRequiredError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.MonedaId = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonedaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyUserIdAfectado_HasRequiredError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UserIdAfectado = string.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserIdAfectado)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_AllFieldsEmpty_HasMultipleErrors()
    {
        // Arrange
        var command = new RegistrarConversionCommand
        {
            CodigoReferido = string.Empty,
            CampaniaCrowdfundingId = Guid.Empty,
            AportacionCrowdfundingId = Guid.Empty,
            ValorMonetario = 0,
            MonedaId = 0,
            UserIdAfectado = string.Empty
        };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(5);
    }

    [Fact]
    public async Task Validate_CodigoReferidoExactly50Chars_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CodigoReferido = new string('A', 50);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CodigoReferido);
    }
}
