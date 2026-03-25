using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Campanias.Queries;

public class GetMisCampaniasQueryHandlerTests
{
    private readonly Mock<ICampaniaService> _serviceMock;
    private readonly Mock<ICrowdFlagsService> _crowdFlagsServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetMisCampaniasQueryHandler>> _loggerMock;
    private readonly GetMisCampaniasQueryHandler _sut;

    public GetMisCampaniasQueryHandlerTests()
    {
        _serviceMock = new Mock<ICampaniaService>();
        _crowdFlagsServiceMock = new Mock<ICrowdFlagsService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetMisCampaniasQueryHandler>>();

        _crowdFlagsServiceMock
            .Setup(x => x.GetFlagsForProyectosAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, CrowdFlags>());

        _sut = new GetMisCampaniasQueryHandler(
            _serviceMock.Object,
            _crowdFlagsServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllCampanias_ForArtista()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var entities = new List<CampaniaCrowdfunding>
        {
            CampaniaTestData.CreateValidBorrador(artistaId),
            CampaniaTestData.CreateValidBorrador(artistaId),
            CampaniaTestData.CreatePublicada(artistaId)
        };

        var expectedDtos = entities.Select(e => new CampaniaListDto
        {
            Id = e.Id.Value,
            ArtistaId = artistaId,
            Titulo = e.Titulo
        }).ToList();

        var query = new GetMisCampaniasQuery { ArtistaId = artistaId };

        _serviceMock
            .Setup(s => s.GetByArtistaIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CampaniaListDto>>(It.IsAny<IEnumerable<CampaniaCrowdfunding>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_FiltersDeletedCampanias()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var activeEntity = CampaniaTestData.CreateValidBorrador(artistaId);
        var deletedEntity = CampaniaTestData.CreateValidBorrador(artistaId);
        deletedEntity.Borrado = true;

        var entities = new List<CampaniaCrowdfunding> { activeEntity, deletedEntity };

        var query = new GetMisCampaniasQuery { ArtistaId = artistaId };

        _serviceMock
            .Setup(s => s.GetByArtistaIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        // The mapper will be called with only the non-deleted entities
        _mapperMock
            .Setup(m => m.Map<IEnumerable<CampaniaListDto>>(It.Is<IEnumerable<CampaniaCrowdfunding>>(
                list => list.All(c => !c.Borrado))))
            .Returns(new List<CampaniaListDto>
            {
                new() { Id = activeEntity.Id.Value, Titulo = activeEntity.Titulo }
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_FiltersByEstado_WhenProvided()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var borrador = CampaniaTestData.CreateValidBorrador(artistaId);
        var publicada = CampaniaTestData.CreatePublicada(artistaId);

        var entities = new List<CampaniaCrowdfunding> { borrador, publicada };

        var query = new GetMisCampaniasQuery
        {
            ArtistaId = artistaId,
            EstadoCampaniaId = 1 // Only Borrador
        };

        _serviceMock
            .Setup(s => s.GetByArtistaIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        // Only borradores should be mapped (EstadoCampaniaId == 1 and not deleted)
        _mapperMock
            .Setup(m => m.Map<IEnumerable<CampaniaListDto>>(It.Is<IEnumerable<CampaniaCrowdfunding>>(
                list => list.All(c => c.EstadoCampaniaId == 1))))
            .Returns(new List<CampaniaListDto>
            {
                new() { Id = borrador.Id.Value, EstadoCampaniaId = 1 }
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data.First().EstadoCampaniaId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_PaginatesCorrectly()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var entities = new List<CampaniaCrowdfunding>();
        for (int i = 0; i < 15; i++)
        {
            var entity = CampaniaTestData.CreateValidBorrador(artistaId);
            entity.FechaCreacion = DateTime.UtcNow.AddMinutes(-i);
            entities.Add(entity);
        }

        var query = new GetMisCampaniasQuery
        {
            ArtistaId = artistaId,
            PageNumber = 2,
            PageSize = 5
        };

        _serviceMock
            .Setup(s => s.GetByArtistaIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        // The handler will Skip(5).Take(5) on the 15 entities
        _mapperMock
            .Setup(m => m.Map<IEnumerable<CampaniaListDto>>(It.Is<IEnumerable<CampaniaCrowdfunding>>(
                list => list.Count() == 5)))
            .Returns(Enumerable.Range(0, 5).Select(i => new CampaniaListDto
            {
                Id = Guid.NewGuid(),
                Titulo = $"Campania {i}"
            }).ToList());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(5);
    }

    [Fact]
    public async Task Handle_MaxPageSize_CappedAt50()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var entities = new List<CampaniaCrowdfunding>();
        for (int i = 0; i < 60; i++)
        {
            var entity = CampaniaTestData.CreateValidBorrador(artistaId);
            entity.FechaCreacion = DateTime.UtcNow.AddMinutes(-i);
            entities.Add(entity);
        }

        var query = new GetMisCampaniasQuery
        {
            ArtistaId = artistaId,
            PageNumber = 1,
            PageSize = 100 // Requesting 100 but should be capped at 50
        };

        _serviceMock
            .Setup(s => s.GetByArtistaIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        // The handler caps at 50
        _mapperMock
            .Setup(m => m.Map<IEnumerable<CampaniaListDto>>(It.Is<IEnumerable<CampaniaCrowdfunding>>(
                list => list.Count() == 50)))
            .Returns(Enumerable.Range(0, 50).Select(i => new CampaniaListDto
            {
                Id = Guid.NewGuid()
            }).ToList());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(50);
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var query = new GetMisCampaniasQuery { ArtistaId = artistaId };

        _serviceMock
            .Setup(s => s.GetByArtistaIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CampaniaCrowdfunding>());

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CampaniaListDto>>(It.IsAny<IEnumerable<CampaniaCrowdfunding>>()))
            .Returns(new List<CampaniaListDto>());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }
}
