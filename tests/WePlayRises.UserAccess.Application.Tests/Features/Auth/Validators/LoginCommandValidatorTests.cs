using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.UserAccess.Application.Features.Auth.Commands;
using WePlayRises.UserAccess.Application.Features.Auth.Validators;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.Application.Tests.Features.Auth.Validators;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _sut;

    public LoginCommandValidatorTests()
    {
        _sut = new LoginCommandValidator();
    }

    private static LoginCommand CreateValidCommand() => new()
    {
        Email = "test@example.com",
        Password = "123456",
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
    public async Task Validate_ValidEmail_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Email = "user@domain.co.uk";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
