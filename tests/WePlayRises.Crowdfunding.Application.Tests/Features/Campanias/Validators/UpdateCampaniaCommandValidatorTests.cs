using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Validators;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Campanias.Validators;

public class UpdateCampaniaCommandValidatorTests
{
    private readonly UpdateCampaniaCommandValidator _sut;

    public UpdateCampaniaCommandValidatorTests()
    {
        _sut = new UpdateCampaniaCommandValidator();
    }

    private static UpdateCampaniaCommand CreateValidCommand() => new()
    {
        Id = Guid.NewGuid(),
        ArtistaId = Guid.NewGuid(),
        Titulo = "Updated Title"
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
    public async Task Validate_TituloTooLong_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Titulo = new string('A', 201);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Titulo)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_InvalidVideoUrl_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.VideoPrincipalUrl = "ftp://not-http-url.com";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.VideoPrincipalUrl)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl);
    }

    [Fact]
    public async Task Validate_NullOptionalFields_ReturnsValid()
    {
        // Arrange
        var command = new UpdateCampaniaCommand
        {
            Id = Guid.NewGuid(),
            ArtistaId = Guid.NewGuid()
            // All optional fields are null
        };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
