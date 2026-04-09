using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Features.Artistas.Queries;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Application.Tests.Helpers;
using WePlayRises.UserAccess.Domain.Constants;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Application.Tests.Features.Artistas.Queries;

public class GetArtistaByUserIdQueryHandlerTests
{
    private readonly Mock<IArtistaService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetArtistaByUserIdQueryHandler>> _loggerMock;
    private readonly GetArtistaByUserIdQueryHandler _sut;

    public GetArtistaByUserIdQueryHandlerTests()
    {
        _serviceMock = new Mock<IArtistaService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetArtistaByUserIdQueryHandler>>();
        _sut = new GetArtistaByUserIdQueryHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AuthorizedAndArtistaExists_ReturnsMappedDto()
    {
        // Arrange
        var userId = "user-123";
        var artistaId = Guid.NewGuid();
        var entity = ArtistaTestData.CreateValid(artistaId, userId);
        var expectedDto = ArtistaTestData.CreateValidDto(artistaId, userId);

        var query = new GetArtistaByUserIdQuery
        {
            UserId = userId,
            RequestingUserId = userId,
        };

        _serviceMock
            .Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(m => m.Map<ArtistaDto>(entity))
            .Returns(expectedDto);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(artistaId);
        result.Data.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task Handle_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var query = new GetArtistaByUserIdQuery
        {
            UserId = "user-123",
            RequestingUserId = "different-user",
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Unauthorized);
    }

    [Fact]
    public async Task Handle_NullRequestingUserId_ReturnsUnauthorized()
    {
        // Arrange
        var query = new GetArtistaByUserIdQuery
        {
            UserId = "user-123",
            RequestingUserId = null,
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Unauthorized);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = "user-123";
        var query = new GetArtistaByUserIdQuery
        {
            UserId = userId,
            RequestingUserId = userId,
        };

        _serviceMock
            .Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Artista);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var userId = "user-123";
        var query = new GetArtistaByUserIdQuery
        {
            UserId = userId,
            RequestingUserId = userId,
        };

        _serviceMock
            .Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB connection lost"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
