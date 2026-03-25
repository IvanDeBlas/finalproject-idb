using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Features.Auth.Commands;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Application.Tests.Helpers;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.Application.Tests.Features.Auth.Commands;

public class LoginCommandHandlerTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
    private readonly Mock<IValidator<LoginCommand>> _validatorMock;
    private readonly Mock<IJwtTokenGenerator> _jwtGeneratorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock;
    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _signInManagerMock = new Mock<SignInManager<IdentityUser>>(
            _userManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
            null!, null!, null!, null!);

        _validatorMock = new Mock<IValidator<LoginCommand>>();
        _jwtGeneratorMock = new Mock<IJwtTokenGenerator>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<LoginCommandHandler>>();

        _sut = new LoginCommandHandler(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _validatorMock.Object,
            _jwtGeneratorMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccessWithToken()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var email = "test@example.com";
        var command = new LoginCommand { Email = email, Password = "123456" };
        var user = AuthTestData.CreateValidIdentityUser(userId, email);
        var roles = new List<string> { "Fan" };
        var token = "jwt-token-123";

        var loginResponse = new LoginResponseDto
        {
            UserId = userId,
            Email = email,
            Token = "",
            Roles = new List<string>(),
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(m => m.CheckPasswordSignInAsync(user, "123456", false))
            .ReturnsAsync(SignInResult.Success);

        _userManagerMock
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(roles);

        _jwtGeneratorMock
            .Setup(g => g.GenerateToken(userId, email, roles))
            .Returns(token);

        _mapperMock
            .Setup(m => m.Map<LoginResponseDto>(user))
            .Returns(loginResponse);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Token.Should().Be(token);
        result.Data.Roles.Should().Contain("Fan");
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = new LoginCommand { Email = "", Password = "" };

        var validationFailures = new List<ValidationFailure>
        {
            new("Email", "El email es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
            new("Password", "La contrasena es obligatoria") { ErrorCode = ServiceResponseMessageType.Validation_Required },
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsInvalidCredentials()
    {
        // Arrange
        var command = new LoginCommand { Email = "unknown@example.com", Password = "123456" };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((IdentityUser?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsInvalidCredentials()
    {
        // Arrange
        var email = "test@example.com";
        var command = new LoginCommand { Email = email, Password = "wrong-password" };
        var user = AuthTestData.CreateValidIdentityUser(email: email);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(m => m.CheckPasswordSignInAsync(user, "wrong-password", false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_InvalidCredentials);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = new LoginCommand { Email = "test@example.com", Password = "123456" };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
