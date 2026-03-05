using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Queries;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Propuestas.Queries;

public class GetNecesidadesPublicasQueryHandlerTests
{
    private readonly Mock<INecesidadCrowdsourcingService> _serviceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetNecesidadesPublicasQueryHandler>> _loggerMock;
    private readonly GetNecesidadesPublicasQueryHandler _sut;

    public GetNecesidadesPublicasQueryHandlerTests()
    {
        _serviceMock = new Mock<INecesidadCrowdsourcingService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetNecesidadesPublicasQueryHandler>>();
        _sut = new GetNecesidadesPublicasQueryHandler(
            _serviceMock.Object,
            _artistaServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithResults_ReturnsPaginatedResponse()
    {
        // Arrange
        var query = new GetNecesidadesPublicasQuery
        {
            UserId = Guid.NewGuid().ToString(),
            Page = 1,
            PageSize = 12
        };

        var necesidad = CrowdsourcingTestData.CreateNecesidadAbierta();
        var items = new List<NecesidadCrowdsourcing> { necesidad };

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        _serviceMock
            .Setup(s => s.GetPublicasPaginatedAsync(
                It.IsAny<NecesidadesPublicasFiltro>(),
                It.IsAny<ArtistaId?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((items as IReadOnlyList<NecesidadCrowdsourcing>, 1));

        var dto = new NecesidadPublicaListDto { Id = necesidad.Id.Value, Titulo = necesidad.Titulo };
        _mapperMock
            .Setup(m => m.Map<List<NecesidadPublicaListDto>>(items))
            .Returns(new List<NecesidadPublicaListDto> { dto });

        _artistaServiceMock
            .Setup(s => s.GetByIdAsync(necesidad.ArtistaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Artista { Id = necesidad.ArtistaId, NombreArtistico = "Test Artist" });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.TotalCount.Should().Be(1);
        result.Data.Page.Should().Be(1);
    }

    [Fact]
    public async Task Handle_EmptyResults_ReturnsEmptyPaginatedResponse()
    {
        // Arrange
        var query = new GetNecesidadesPublicasQuery
        {
            UserId = Guid.NewGuid().ToString(),
            Page = 1,
            PageSize = 12
        };

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        var emptyList = new List<NecesidadCrowdsourcing>();
        _serviceMock
            .Setup(s => s.GetPublicasPaginatedAsync(
                It.IsAny<NecesidadesPublicasFiltro>(),
                It.IsAny<ArtistaId?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((emptyList as IReadOnlyList<NecesidadCrowdsourcing>, 0));

        _mapperMock
            .Setup(m => m.Map<List<NecesidadPublicaListDto>>(emptyList))
            .Returns(new List<NecesidadPublicaListDto>());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetNecesidadesPublicasQuery
        {
            UserId = Guid.NewGuid().ToString(),
            Page = 1,
            PageSize = 12
        };

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_WithArtista_ExcludesOwnNecesidades()
    {
        // Arrange
        var artistaId = new ArtistaId(Guid.NewGuid());
        var query = new GetNecesidadesPublicasQuery
        {
            UserId = Guid.NewGuid().ToString(),
            Page = 1,
            PageSize = 12
        };

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Artista { Id = artistaId, NombreArtistico = "My Artist" });

        var emptyList = new List<NecesidadCrowdsourcing>();
        _serviceMock
            .Setup(s => s.GetPublicasPaginatedAsync(
                It.IsAny<NecesidadesPublicasFiltro>(),
                artistaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((emptyList as IReadOnlyList<NecesidadCrowdsourcing>, 0));

        _mapperMock
            .Setup(m => m.Map<List<NecesidadPublicaListDto>>(emptyList))
            .Returns(new List<NecesidadPublicaListDto>());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.GetPublicasPaginatedAsync(
            It.IsAny<NecesidadesPublicasFiltro>(),
            artistaId,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
