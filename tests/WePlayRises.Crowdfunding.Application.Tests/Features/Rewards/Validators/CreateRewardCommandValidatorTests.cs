using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Validators;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Rewards.Validators;

public class CreateRewardCommandValidatorTests
{
    private readonly CreateRewardCommandValidator _sut;

    public CreateRewardCommandValidatorTests()
    {
        _sut = new CreateRewardCommandValidator();
    }

    private static CreateRewardCommand CreateValidCommand() => new()
    {
        CampaniaId = Guid.NewGuid(),
        TipoRewardId = 1,
        Nombre = "Test Reward",
        Descripcion = "Test description",
        ImporteMinimo = 25m,
        MonedaId = 1,
        EsAddOn = false,
        IncluyeEnvioFisico = false,
        Orden = 1
    };

    [Fact]
    public async Task Validate_ValidCommand_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_EmptyCampaniaId_HasError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CampaniaId = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.CampaniaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyNombre_HasError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Nombre = "";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Nombre)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_NombreTooLong_HasError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Nombre = new string('A', 201);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Nombre)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_DescripcionTooLong_HasError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Descripcion = new string('A', 2001);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Descripcion)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
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
    public async Task Validate_ZeroImporteMinimo_HasError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImporteMinimo = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ImporteMinimo)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Validate_NegativeImporteMinimo_HasError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImporteMinimo = -10m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ImporteMinimo)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Validate_ZeroMonedaId_HasError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.MonedaId = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.MonedaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_ZeroTipoRewardId_HasError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TipoRewardId = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.TipoRewardId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_ZeroCantidadMaxima_HasError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CantidadMaxima = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.CantidadMaxima)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Fact]
    public async Task Validate_NullCantidadMaxima_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CantidadMaxima = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CantidadMaxima);
    }

    [Fact]
    public async Task Validate_ValidCantidadMaxima_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CantidadMaxima = 100;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CantidadMaxima);
    }

    [Fact]
    public async Task Validate_ZeroCantidadPorBacker_HasError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CantidadPorBacker = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.CantidadPorBacker)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Fact]
    public async Task Validate_NullCantidadPorBacker_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CantidadPorBacker = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CantidadPorBacker);
    }

    [Fact]
    public async Task Validate_TiempoEntregaTooLong_HasError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TiempoEntregaEstimado = new string('A', 201);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.TiempoEntregaEstimado)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_NullTiempoEntrega_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TiempoEntregaEstimado = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TiempoEntregaEstimado);
    }

    [Fact]
    public async Task Validate_NegativeOrden_HasError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Orden = -1;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Orden)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Fact]
    public async Task Validate_ZeroOrden_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Orden = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Orden);
    }
}
