using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Validators;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Promotor.Validators;

public class UpdatePromotorCommandValidatorTests
{
    private readonly UpdatePromotorCommandValidator _sut;

    public UpdatePromotorCommandValidatorTests()
    {
        _sut = new UpdatePromotorCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = PromotorTestData.CreateValidUpdateCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyNombrePublico_ShouldHaveError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidUpdateCommand();
        command.NombrePublico = "";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NombrePublico)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_ShortNombrePublico_ShouldHaveError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidUpdateCommand();
        command.NombrePublico = "ab";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NombrePublico)
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength);
    }

    [Fact]
    public async Task Validate_InvalidEmail_ShouldHaveError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidUpdateCommand();
        command.EmailContacto = "invalid-email";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmailContacto)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidEmail);
    }

    [Fact]
    public async Task Validate_InvalidUrl_ShouldHaveError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidUpdateCommand();
        command.UrlInstagram = "not-a-valid-url";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UrlInstagram)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl);
    }

    [Fact]
    public async Task Validate_EmptyUserId_ShouldHaveError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidUpdateCommand();
        command.UserId = "";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_NullOptionalFields_ShouldNotHaveErrors()
    {
        // Arrange
        var command = PromotorTestData.CreateValidUpdateCommand();
        command.EmailContacto = null;
        command.UrlSitioWeb = null;
        command.UrlInstagram = null;
        command.UrlTikTok = null;
        command.UrlYouTube = null;
        command.UrlTwitter = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
