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

public class GetAllCampaniasQueryHandlerTests
{
    private readonly Mock<ICampaniaService> _serviceMock;
    private readonly Mock<ICrowdFlagsService> _crowdFlagsServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetAllCampaniasQueryHandler>> _loggerMock;
    private readonly GetAllCampaniasQueryHandler _sut;

    public GetAllCampaniasQueryHandlerTests()
    {
        _serviceMock = new Mock<ICampaniaService>();
        _crowdFlagsServiceMock = new Mock<ICrowdFlagsService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetAllCampaniasQueryHandler>>();

        _crowdFlagsServiceMock
            .Setup(x => x.GetFlagsForProyectosAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, CrowdFlags>());

        _sut = new GetAllCampaniasQueryHandler(
            _serviceMock.Object,
            _crowdFlagsServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllCampanias()
    {
        // Arrange
        var entities = new List<CampaniaCrowdfunding>
        {
            CampaniaTestData.CreateValidBorrador(),
            CampaniaTestData.CreatePublicada()
        };

        var expectedDtos = entities.Select(e => new CampaniaListDto
        {
            Id = e.Id.Value,
            Titulo = e.Titulo
        }).ToList();

        var query = new GetAllCampaniasQuery();

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CampaniaListDto>>(It.IsAny<IEnumerable<CampaniaCrowdfunding>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        _serviceMock.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FilterByArtistaId_UsesGetByArtistaId()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreateValidBorrador(artistaId);
        var entities = new List<CampaniaCrowdfunding> { entity };

        var query = new GetAllCampaniasQuery { ArtistaId = artistaId };

        _serviceMock
            .Setup(s => s.GetByArtistaIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CampaniaListDto>>(It.IsAny<IEnumerable<CampaniaCrowdfunding>>()))
            .Returns(new List<CampaniaListDto>
            {
                new() { Id = entity.Id.Value, ArtistaId = artistaId }
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        _serviceMock.Verify(s => s.GetByArtistaIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()), Times.Once);
        _serviceMock.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_FilterByEstado_ReturnsOnlyMatchingEstado()
    {
        // Arrange
        var borrador = CampaniaTestData.CreateValidBorrador();
        var publicada = CampaniaTestData.CreatePublicada();
        var entities = new List<CampaniaCrowdfunding> { borrador, publicada };

        var query = new GetAllCampaniasQuery { EstadoCampaniaId = 2 };

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CampaniaListDto>>(It.Is<IEnumerable<CampaniaCrowdfunding>>(
                list => list.All(c => c.EstadoCampaniaId == 2))))
            .Returns(new List<CampaniaListDto>
            {
                new() { Id = publicada.Id.Value, EstadoCampaniaId = 2 }
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ExcludesBorradoCampanias()
    {
        // Arrange
        var active = CampaniaTestData.CreateValidBorrador();
        var deleted = CampaniaTestData.CreateValidBorrador();
        deleted.Borrado = true;
        var entities = new List<CampaniaCrowdfunding> { active, deleted };

        var query = new GetAllCampaniasQuery();

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CampaniaListDto>>(It.Is<IEnumerable<CampaniaCrowdfunding>>(
                list => list.All(c => !c.Borrado))))
            .Returns(new List<CampaniaListDto>
            {
                new() { Id = active.Id.Value }
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_SearchTerm_FiltersMatchingEntities()
    {
        // Arrange
        var matching = CampaniaTestData.CreateValidBorrador();
        matching.Titulo = "Rock Concert Campaign";
        var nonMatching = CampaniaTestData.CreateValidBorrador();
        nonMatching.Titulo = "Jazz Album";
        var entities = new List<CampaniaCrowdfunding> { matching, nonMatching };

        var query = new GetAllCampaniasQuery { SearchTerm = "Rock" };

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CampaniaListDto>>(It.Is<IEnumerable<CampaniaCrowdfunding>>(
                list => list.Count() == 1)))
            .Returns(new List<CampaniaListDto>
            {
                new() { Id = matching.Id.Value, Titulo = "Rock Concert Campaign" }
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Pagination_ReturnsPagedResults()
    {
        // Arrange
        var entities = new List<CampaniaCrowdfunding>();
        for (int i = 0; i < 10; i++)
        {
            entities.Add(CampaniaTestData.CreateValidBorrador());
        }

        var query = new GetAllCampaniasQuery { PageNumber = 2, PageSize = 3 };

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CampaniaListDto>>(It.Is<IEnumerable<CampaniaCrowdfunding>>(
                list => list.Count() == 3)))
            .Returns(Enumerable.Range(0, 3).Select(_ => new CampaniaListDto
            {
                Id = Guid.NewGuid()
            }).ToList());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllCampaniasQuery();

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
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

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetAllCampaniasQuery();

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
