using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Validators;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Promotor.Validators;

public class CreatePromotorCommandValidatorTests
{
    private readonly Mock<IPromotorService> _serviceMock;
    private readonly CreatePromotorCommandValidator _sut;

    public CreatePromotorCommandValidatorTests()
    {
        _serviceMock = new Mock<IPromotorService>();
        _serviceMock.Setup(s => s.TipoPromotorExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _sut = new CreatePromotorCommandValidator(_serviceMock.Object);
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = PromotorTestData.CreateValidCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyNombrePublico_ShouldHaveError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidCommand();
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
        var command = PromotorTestData.CreateValidCommand();
        command.NombrePublico = "ab";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NombrePublico)
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength);
    }

    [Fact]
    public async Task Validate_LongNombrePublico_ShouldHaveError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidCommand();
        command.NombrePublico = new string('A', 201);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NombrePublico)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_ZeroTipoPromotor_ShouldHaveError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidCommand();
        command.TipoPromotorId = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TipoPromotorId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_InvalidTipoPromotor_ShouldHaveError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidCommand();
        command.TipoPromotorId = 99;

        _serviceMock.Setup(s => s.TipoPromotorExistsAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TipoPromotorId)
            .WithErrorCode(ServiceResponseMessageType.Validation_ForeignKeyNotFound);
    }

    [Fact]
    public async Task Validate_InvalidEmail_ShouldHaveError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidCommand();
        command.EmailContacto = "not-an-email";

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
        var command = PromotorTestData.CreateValidCommand();
        command.UrlSitioWeb = "not-a-url";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UrlSitioWeb)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl);
    }

    [Fact]
    public async Task Validate_EmptyUserId_ShouldHaveError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidCommand();
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
        var command = PromotorTestData.CreateValidCommand();
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
