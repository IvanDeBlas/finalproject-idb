using FluentValidation.TestHelper;
using WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands;
using WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Validators;
using WePlayRises.Crowdpromotion.Domain.Constants;
using Xunit;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.PromoTarea.Validators;

public class RechazarTareaCommandValidatorTests
{
    private readonly RechazarTareaCommandValidator _sut;

    public RechazarTareaCommandValidatorTests()
    {
        _sut = new RechazarTareaCommandValidator();
    }

    private static RechazarTareaCommand CreateValidCommand()
    {
        return new RechazarTareaCommand
        {
            ProgramaId = Guid.NewGuid(),
            TareaPromotorId = Guid.NewGuid(),
            ComentarioValidacion = "No cumple los requisitos",
            UserId = Guid.NewGuid().ToString()
        };
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ShouldHaveError()
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
    public async Task Validate_EmptyProgramaId_ShouldHaveError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ProgramaId = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProgramaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyTareaPromotorId_ShouldHaveError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TareaPromotorId = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TareaPromotorId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyComentario_ShouldHaveError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ComentarioValidacion = string.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ComentarioValidacion)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_ComentarioTooLong_ShouldHaveError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ComentarioValidacion = new string('x', 501);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ComentarioValidacion)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_NullComentario_ShouldHaveError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ComentarioValidacion = null!;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ComentarioValidacion)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
