using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.UserAccess.Application.Features.Auth.Commands;
using WePlayRises.UserAccess.Application.Features.Auth.Validators;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.Application.Tests.Features.Auth.Validators;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _sut;

    public RegisterCommandValidatorTests()
    {
        _sut = new RegisterCommandValidator();
    }

    private static RegisterCommand CreateValidCommand() => new()
    {
        Email = "newuser@example.com",
        Password = "123456",
        ConfirmPassword = "123456",
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
    public async Task Validate_EmptyEmail_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Email = "";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_InvalidEmailFormat_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Email = "not-an-email";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidEmail);
    }

    [Fact]
    public async Task Validate_EmptyPassword_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Password = "";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_PasswordTooShort_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Password = "12345";
        command.ConfirmPassword = "12345";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength);
    }

    [Fact]
    public async Task Validate_EmptyConfirmPassword_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ConfirmPassword = "";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_PasswordsDoNotMatch_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Password = "123456";
        command.ConfirmPassword = "654321";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidFormat);
    }

    [Fact]
    public async Task Validate_InvalidRole_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Role = "InvalidRole";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Role)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidFormat);
    }

    [Fact]
    public async Task Validate_ValidRole_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Role = Roles.Fan;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_NullRole_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Role = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ArtistaRole_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Role = Roles.Artista;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
