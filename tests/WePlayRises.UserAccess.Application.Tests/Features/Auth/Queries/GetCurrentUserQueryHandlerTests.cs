using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Features.Auth.Queries;
using WePlayRises.UserAccess.Application.Tests.Helpers;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.Application.Tests.Features.Auth.Queries;

public class GetCurrentUserQueryHandlerTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetCurrentUserQueryHandler>> _loggerMock;
    private readonly GetCurrentUserQueryHandler _sut;

    public GetCurrentUserQueryHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetCurrentUserQueryHandler>>();

        _sut = new GetCurrentUserQueryHandler(
            _userManagerMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_UserExists_ReturnsUserInfoDto()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var email = "test@example.com";
        var user = AuthTestData.CreateValidIdentityUser(userId, email);
        var roles = new List<string> { "Fan" };
        var expectedDto = AuthTestData.CreateUserInfoDto(userId, email);

        var query = new GetCurrentUserQuery { UserId = userId };

        _userManagerMock
            .Setup(m => m.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(roles);

        _mapperMock
            .Setup(m => m.Map<UserInfoDto>(user))
            .Returns(expectedDto);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.UserId.Should().Be(userId);
        result.Data.Email.Should().Be(email);
        result.Data.Roles.Should().Contain("Fan");
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var query = new GetCurrentUserQuery { UserId = userId };

        _userManagerMock
            .Setup(m => m.FindByIdAsync(userId))
            .ReturnsAsync((IdentityUser?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_User);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var query = new GetCurrentUserQuery { UserId = userId };

        _userManagerMock
            .Setup(m => m.FindByIdAsync(userId))
            .ThrowsAsync(new Exception("DB connection lost"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
