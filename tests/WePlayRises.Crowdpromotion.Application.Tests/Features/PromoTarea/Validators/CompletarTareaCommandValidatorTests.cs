using FluentValidation.TestHelper;
using WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands;
using WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Validators;
using WePlayRises.Crowdpromotion.Domain.Constants;
using Xunit;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.PromoTarea.Validators;

public class CompletarTareaCommandValidatorTests
{
    private readonly CompletarTareaCommandValidator _sut;

    public CompletarTareaCommandValidatorTests()
    {
        _sut = new CompletarTareaCommandValidator();
    }

    private static CompletarTareaCommand CreateValidCommand()
    {
        return new CompletarTareaCommand
        {
            ProgramaId = Guid.NewGuid(),
            TareaId = Guid.NewGuid(),
            UrlPruebaCompletado = "https://example.com/proof",
            ComentarioPromotor = "Completed",
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
    public async Task Validate_EmptyTareaId_ShouldHaveError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TareaId = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TareaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyUrl_ShouldHaveError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UrlPruebaCompletado = string.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UrlPruebaCompletado)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_UrlTooLong_ShouldHaveError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UrlPruebaCompletado = "https://example.com/" + new string('a', 2030);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UrlPruebaCompletado)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_InvalidUrl_ShouldHaveError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UrlPruebaCompletado = "not-a-valid-url";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UrlPruebaCompletado)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl);
    }

    [Fact]
    public async Task Validate_ComentarioTooLong_ShouldHaveError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ComentarioPromotor = new string('x', 501);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ComentarioPromotor)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_NullComentario_ShouldNotHaveError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ComentarioPromotor = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ComentarioPromotor);
    }
}
