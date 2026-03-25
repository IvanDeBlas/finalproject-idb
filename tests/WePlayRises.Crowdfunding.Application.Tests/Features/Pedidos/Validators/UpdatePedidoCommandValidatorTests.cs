using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdfunding.Application.Features.Pedidos.Commands;
using WePlayRises.Crowdfunding.Application.Features.Pedidos.Validators;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Pedidos.Validators;

public class UpdatePedidoCommandValidatorTests
{
    private readonly UpdatePedidoCommandValidator _sut;

    public UpdatePedidoCommandValidatorTests()
    {
        _sut = new UpdatePedidoCommandValidator();
    }

    private static UpdatePedidoCommand CreateValidCommand() => new()
    {
        Id = Guid.NewGuid(),
        EstadoPedidoId = 2,
        ImportePropina = 10m,
        ComentarioBacker = "Updated comment"
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
    public async Task Validate_EmptyId_ReturnsError()
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
    public async Task Validate_ZeroEstadoPedidoId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.EstadoPedidoId = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.EstadoPedidoId)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidFormat);
    }

    [Fact]
    public async Task Validate_NegativeEstadoPedidoId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.EstadoPedidoId = -1;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.EstadoPedidoId)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidFormat);
    }

    [Fact]
    public async Task Validate_NullEstadoPedidoId_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.EstadoPedidoId = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_NegativeImportePropina_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImportePropina = -5m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ImportePropina)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Validate_NullImportePropina_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImportePropina = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ComentarioTooLong_ReturnsMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ComentarioBacker = new string('B', 501);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ComentarioBacker)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
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

    [Fact]
    public async Task Validate_OnlyIdProvided_ReturnsValid()
    {
        // Arrange
        var command = new UpdatePedidoCommand
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
