using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Features.Artistas.Queries;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Application.Tests.Helpers;
using WePlayRises.UserAccess.Domain.Constants;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Application.Tests.Features.Artistas.Queries;

public class GetArtistaByIdQueryHandlerTests
{
    private readonly Mock<IArtistaService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetArtistaByIdQueryHandler>> _loggerMock;
    private readonly GetArtistaByIdQueryHandler _sut;

    public GetArtistaByIdQueryHandlerTests()
    {
        _serviceMock = new Mock<IArtistaService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetArtistaByIdQueryHandler>>();
        _sut = new GetArtistaByIdQueryHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ArtistaExists_ReturnsMappedDto()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var userId = "user-123";
        var entity = ArtistaTestData.CreateValid(artistaId, userId);
        var expectedDto = ArtistaTestData.CreateValidDto(artistaId, userId);

        var query = new GetArtistaByIdQuery { Id = artistaId };

        _serviceMock
            .Setup(s => s.GetByIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
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
        result.Data.NombreArtistico.Should().Be("Test Artist");
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var query = new GetArtistaByIdQuery { Id = artistaId };

        _serviceMock
            .Setup(s => s.GetByIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
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
        var artistaId = Guid.NewGuid();
        var query = new GetArtistaByIdQuery { Id = artistaId };

        _serviceMock
            .Setup(s => s.GetByIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB connection lost"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
