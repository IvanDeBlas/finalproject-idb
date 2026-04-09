using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Commands;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Validators;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Wallet.Validators;

public class SolicitarCobroCommandValidatorTests
{
    private readonly SolicitarCobroCommandValidator _sut;

    public SolicitarCobroCommandValidatorTests()
    {
        _sut = new SolicitarCobroCommandValidator();
    }

    private static SolicitarCobroCommand CreateValidCommand()
    {
        return new SolicitarCobroCommand
        {
            UserId = Guid.NewGuid().ToString(),
            Importe = 25.00m,
            Descripcion = null
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
    public async Task Validate_ValidCommandWithDescripcion_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Descripcion = "Retiro mensual de comisiones";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_HasRequiredError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UserId = string.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_ImporteZero_HasRangeError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Importe = 0m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Importe)
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);
    }

    [Fact]
    public async Task Validate_NegativeImporte_HasRangeError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Importe = -10.50m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Importe)
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);
    }

    [Fact]
    public async Task Validate_SmallPositiveImporte_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Importe = 0.01m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Importe);
    }

    [Fact]
    public async Task Validate_DescripcionTooLong_HasMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Descripcion = new string('X', 501);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Descripcion)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_DescripcionExactly500_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Descripcion = new string('A', 500);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Descripcion);
    }

    [Fact]
    public async Task Validate_NullDescripcion_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Descripcion = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Descripcion);
    }

    [Fact]
    public async Task Validate_AllFieldsInvalid_HasMultipleErrors()
    {
        // Arrange
        var command = new SolicitarCobroCommand
        {
            UserId = string.Empty,
            Importe = -5m,
            Descripcion = new string('X', 501)
        };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(3);
    }
}
