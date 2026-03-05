using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Queries;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Necesidades.Queries;

public class GetMisNecesidadesQueryHandlerTests
{
    private readonly Mock<INecesidadCrowdsourcingService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetMisNecesidadesQueryHandler>> _loggerMock;
    private readonly GetMisNecesidadesQueryHandler _sut;

    public GetMisNecesidadesQueryHandlerTests()
    {
        _serviceMock = new Mock<INecesidadCrowdsourcingService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetMisNecesidadesQueryHandler>>();
        _sut = new GetMisNecesidadesQueryHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithData_ReturnsPaginatedResults()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var necesidades = new List<NecesidadCrowdsourcing>
        {
            CrowdsourcingTestData.CreateNecesidad(artistaId: artistaId),
            CrowdsourcingTestData.CreateNecesidad(artistaId: artistaId)
        };

        var dtos = new List<NecesidadCrowdsourcingListDto>
        {
            new() { Id = necesidades[0].Id.Value, Titulo = necesidades[0].Titulo },
            new() { Id = necesidades[1].Id.Value, Titulo = necesidades[1].Titulo }
        };

        _serviceMock
            .Setup(s => s.GetByArtistaIdPaginatedAsync(
                It.IsAny<ArtistaId>(), null, null, 1, 12, It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<NecesidadCrowdsourcing>)necesidades, 2));

        _mapperMock
            .Setup(m => m.Map<List<NecesidadCrowdsourcingListDto>>(necesidades))
            .Returns(dtos);

        var query = new GetMisNecesidadesQuery { ArtistaId = artistaId };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
        result.Data.Page.Should().Be(1);
        result.Data.PageSize.Should().Be(12);
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResponse()
    {
        // Arrange
        var artistaId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.GetByArtistaIdPaginatedAsync(
                It.IsAny<ArtistaId>(), null, null, 1, 12, It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<NecesidadCrowdsourcing>)new List<NecesidadCrowdsourcing>(), 0));

        _mapperMock
            .Setup(m => m.Map<List<NecesidadCrowdsourcingListDto>>(It.IsAny<IReadOnlyList<NecesidadCrowdsourcing>>()))
            .Returns(new List<NecesidadCrowdsourcingListDto>());

        var query = new GetMisNecesidadesQuery { ArtistaId = artistaId };

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
        var query = new GetMisNecesidadesQuery { ArtistaId = Guid.NewGuid() };

        _serviceMock
            .Setup(s => s.GetByArtistaIdPaginatedAsync(
                It.IsAny<ArtistaId>(), It.IsAny<int?>(), It.IsAny<string?>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_WithFilters_PassesFiltersToService()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var query = new GetMisNecesidadesQuery
        {
            ArtistaId = artistaId,
            EstadoNecesidadId = 1,
            Search = "mezcla",
            Page = 2,
            PageSize = 6
        };

        _serviceMock
            .Setup(s => s.GetByArtistaIdPaginatedAsync(
                It.Is<ArtistaId>(a => a.Value == artistaId),
                1, "mezcla", 2, 6,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<NecesidadCrowdsourcing>)new List<NecesidadCrowdsourcing>(), 0));

        _mapperMock
            .Setup(m => m.Map<List<NecesidadCrowdsourcingListDto>>(It.IsAny<IReadOnlyList<NecesidadCrowdsourcing>>()))
            .Returns(new List<NecesidadCrowdsourcingListDto>());

        // Act
        await _sut.Handle(query, CancellationToken.None);

        // Assert
        _serviceMock.Verify(s => s.GetByArtistaIdPaginatedAsync(
            It.Is<ArtistaId>(a => a.Value == artistaId),
            1, "mezcla", 2, 6,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
