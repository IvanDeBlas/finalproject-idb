using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdfunding.Application.Features.Pedidos.Commands;
using WePlayRises.Crowdfunding.Application.Features.Pedidos.Validators;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Pedidos.Validators;

public class CreatePedidoCommandValidatorTests
{
    private readonly CreatePedidoCommandValidator _sut;

    public CreatePedidoCommandValidatorTests()
    {
        _sut = new CreatePedidoCommandValidator();
    }

    private static CreatePedidoCommand CreateValidCommand() => new()
    {
        CampaniaId = Guid.NewGuid(),
        MonedaId = 1,
        ImporteSubtotal = 45m,
        ImportePropina = 5m,
        ImporteEnvio = 0m,
        ImporteImpuestos = 0m,
        ImporteTotal = 50m,
        PermitirMostrarNombre = true,
        ComentarioBacker = "Great project!"
    };

    [Fact]
    public async Task Validate_ValidCommand_ReturnsValid()
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
    public async Task Validate_EmptyCampaniaId_ReturnsError()
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
    public async Task Validate_ZeroMonedaId_ReturnsError()
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
    public async Task Validate_NegativeImporteSubtotal_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImporteSubtotal = -1m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ImporteSubtotal)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Validate_NegativeImportePropina_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImportePropina = -1m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ImportePropina)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Validate_NegativeImporteEnvio_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImporteEnvio = -1m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ImporteEnvio)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Validate_NegativeImporteImpuestos_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImporteImpuestos = -1m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ImporteImpuestos)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Validate_ZeroImporteTotal_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImporteTotal = 0m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ImporteTotal)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Validate_NegativeImporteTotal_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImporteTotal = -10m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ImporteTotal)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Validate_ComentarioTooLong_ReturnsMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ComentarioBacker = new string('A', 501);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ComentarioBacker)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_ComentarioExactly500_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ComentarioBacker = new string('A', 500);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_NullComentario_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ComentarioBacker = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
