using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
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

public class RegisterCommandHandlerTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<IValidator<RegisterCommand>> _validatorMock;
    private readonly Mock<IJwtTokenGenerator> _jwtGeneratorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<RegisterCommandHandler>> _loggerMock;
    private readonly RegisterCommandHandler _sut;

    public RegisterCommandHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _validatorMock = new Mock<IValidator<RegisterCommand>>();
        _jwtGeneratorMock = new Mock<IJwtTokenGenerator>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<RegisterCommandHandler>>();

        _sut = new RegisterCommandHandler(
            _userManagerMock.Object,
            _validatorMock.Object,
            _jwtGeneratorMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithToken()
    {
        // Arrange
        var email = "newuser@example.com";
        var command = new RegisterCommand
        {
            Email = email,
            Password = "123456",
            ConfirmPassword = "123456",
        };

        var roles = new List<string> { "Fan" };
        var token = "jwt-token-123";

        var registerResponse = new RegisterResponseDto
        {
            UserId = "",
            Email = email,
            Token = "",
            Roles = new List<string>(),
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(email))
            .ReturnsAsync((IdentityUser?)null);

        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), command.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), Roles.Fan))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(m => m.GetRolesAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(roles);

        _jwtGeneratorMock
            .Setup(g => g.GenerateToken(It.IsAny<string>(), email, roles))
            .Returns(token);

        _mapperMock
            .Setup(m => m.Map<RegisterResponseDto>(It.IsAny<IdentityUser>()))
            .Returns(registerResponse);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Token.Should().Be(token);
        result.Data.Roles.Should().Contain("Fan");
        _userManagerMock.Verify(m => m.CreateAsync(It.IsAny<IdentityUser>(), command.Password), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "",
            Password = "",
            ConfirmPassword = "",
        };

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
        _userManagerMock.Verify(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_EmailAlreadyExists_ReturnsConflict()
    {
        // Arrange
        var email = "existing@example.com";
        var command = new RegisterCommand
        {
            Email = email,
            Password = "123456",
            ConfirmPassword = "123456",
        };

        var existingUser = AuthTestData.CreateValidIdentityUser(email: email);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Validation_DuplicateEmail);
        _userManagerMock.Verify(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_IdentityCreateFails_ReturnsErrors()
    {
        // Arrange
        var email = "newuser@example.com";
        var command = new RegisterCommand
        {
            Email = email,
            Password = "weak",
            ConfirmPassword = "weak",
        };

        var identityErrors = new[]
        {
            new IdentityError { Code = "PasswordTooShort", Description = "Passwords must be at least 6 characters." },
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(email))
            .ReturnsAsync((IdentityUser?)null);

        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), command.Password))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_WithCustomRole_AssignsSpecifiedRole()
    {
        // Arrange
        var email = "artist@example.com";
        var command = new RegisterCommand
        {
            Email = email,
            Password = "123456",
            ConfirmPassword = "123456",
            Role = Roles.Artista,
        };

        var roles = new List<string> { Roles.Artista };
        var token = "jwt-token-123";
        var registerResponse = new RegisterResponseDto
        {
            UserId = "",
            Email = email,
            Token = "",
            Roles = new List<string>(),
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(email))
            .ReturnsAsync((IdentityUser?)null);

        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), command.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), Roles.Artista))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(m => m.GetRolesAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(roles);

        _jwtGeneratorMock
            .Setup(g => g.GenerateToken(It.IsAny<string>(), email, roles))
            .Returns(token);

        _mapperMock
            .Setup(m => m.Map<RegisterResponseDto>(It.IsAny<IdentityUser>()))
            .Returns(registerResponse);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), Roles.Artista), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "test@example.com",
            Password = "123456",
            ConfirmPassword = "123456",
        };

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
