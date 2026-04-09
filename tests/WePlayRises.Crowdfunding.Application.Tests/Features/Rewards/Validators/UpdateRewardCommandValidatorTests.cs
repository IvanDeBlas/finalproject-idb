using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Validators;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Rewards.Validators;

public class UpdateRewardCommandValidatorTests
{
    private readonly UpdateRewardCommandValidator _sut;

    public UpdateRewardCommandValidatorTests()
    {
        _sut = new UpdateRewardCommandValidator();
    }

    private static UpdateRewardCommand CreateValidCommand() => new()
    {
        Id = Guid.NewGuid(),
        Nombre = "Updated Reward",
        Descripcion = "Updated description",
        ImporteMinimo = 50m
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
    public async Task Validate_MinimalValidCommand_IsValid()
    {
        // Arrange - Only Id is required, all other fields are optional
        var command = new UpdateRewardCommand { Id = Guid.NewGuid() };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyId_HasError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Id = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id)
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
    public async Task Validate_NullNombre_IsValid()
    {
        // Arrange
        var command = new UpdateRewardCommand { Id = Guid.NewGuid(), Nombre = null };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Nombre);
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
        var command = new UpdateRewardCommand { Id = Guid.NewGuid(), Descripcion = null };

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
        command.ImporteMinimo = -5m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ImporteMinimo)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Validate_NullImporteMinimo_IsValid()
    {
        // Arrange
        var command = new UpdateRewardCommand { Id = Guid.NewGuid(), ImporteMinimo = null };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ImporteMinimo);
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
        var command = new UpdateRewardCommand { Id = Guid.NewGuid(), CantidadMaxima = null };

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
        var command = new UpdateRewardCommand { Id = Guid.NewGuid(), CantidadPorBacker = null };

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
        var command = new UpdateRewardCommand { Id = Guid.NewGuid(), TiempoEntregaEstimado = null };

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
    public async Task Validate_NullOrden_IsValid()
    {
        // Arrange
        var command = new UpdateRewardCommand { Id = Guid.NewGuid(), Orden = null };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Orden);
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
