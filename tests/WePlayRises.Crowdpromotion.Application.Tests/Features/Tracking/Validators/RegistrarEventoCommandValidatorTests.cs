using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Commands;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Validators;
using WePlayRises.Crowdpromotion.Domain.Constants;
using Xunit;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Tracking.Validators;

public class RegistrarEventoCommandValidatorTests
{
    private readonly RegistrarEventoCommandValidator _sut;

    public RegistrarEventoCommandValidatorTests()
    {
        _sut = new RegistrarEventoCommandValidator();
    }

    private static RegistrarEventoCommand CreateValidCommand()
    {
        return new RegistrarEventoCommand
        {
            TipoEventoPromoId = 1, // Click
            CodigoReferido = "ABC123",
            CampaniaCrowdfundingId = Guid.NewGuid(),
            UrlOrigen = "https://example.com/campania",
            UrlReferer = "https://google.com",
            UtmSource = "twitter",
            UtmMedium = "social",
            UtmCampaign = "summer2026",
            IpOrigen = "192.168.1.1",
            UserIdAfectado = Guid.NewGuid().ToString()
        };
    }

    [Fact]
    public async Task Validate_ValidClickCommand_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_TipoEventoZero_HasRequiredError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TipoEventoPromoId = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TipoEventoPromoId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_TipoEventoOutOfRange_HasInvalidError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TipoEventoPromoId = 99;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TipoEventoPromoId)
            .WithErrorCode(ServiceResponseMessageType.Validation_TipoEventoInvalido);
    }

    [Fact]
    public async Task Validate_TipoEventoBacking_HasBackingNotAllowedError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TipoEventoPromoId = 4; // Backing

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TipoEventoPromoId)
            .WithErrorCode(ServiceResponseMessageType.Validation_BackingNoPermitido);
    }

    [Fact]
    public async Task Validate_CodigoReferidoTooLong_HasMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CodigoReferido = new string('A', 51);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CodigoReferido)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_UrlOrigenTooLong_HasMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UrlOrigen = "https://example.com/" + new string('a', 2030);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UrlOrigen)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_UrlRefererTooLong_HasMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UrlReferer = "https://example.com/" + new string('r', 2030);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UrlReferer)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_UtmSourceTooLong_HasMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UtmSource = new string('s', 101);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UtmSource)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_UtmMediumTooLong_HasMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UtmMedium = new string('m', 101);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UtmMedium)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_UtmCampaignTooLong_HasMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UtmCampaign = new string('c', 101);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UtmCampaign)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_NegativeTipoEvento_HasRequiredError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TipoEventoPromoId = -1;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TipoEventoPromoId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Theory]
    [InlineData(1)] // Click
    [InlineData(2)] // PageView
    [InlineData(3)] // Signup
    [InlineData(5)] // Share
    public async Task Validate_AllowedTipoEventoValues_IsValid(int tipoEvento)
    {
        // Arrange
        var command = CreateValidCommand();
        command.TipoEventoPromoId = tipoEvento;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TipoEventoPromoId);
    }

    [Fact]
    public async Task Validate_NullOptionalFields_IsValid()
    {
        // Arrange
        var command = new RegistrarEventoCommand
        {
            TipoEventoPromoId = 1,
            CodigoReferido = null,
            CampaniaCrowdfundingId = null,
            UrlOrigen = null,
            UrlReferer = null,
            UtmSource = null,
            UtmMedium = null,
            UtmCampaign = null,
            IpOrigen = null,
            UserIdAfectado = null
        };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyStringOptionalFields_IsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CodigoReferido = string.Empty;
        command.UrlOrigen = string.Empty;
        command.UrlReferer = string.Empty;
        command.UtmSource = string.Empty;
        command.UtmMedium = string.Empty;
        command.UtmCampaign = string.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
