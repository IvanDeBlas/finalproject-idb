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

public class GetCampaniaStatsQueryHandlerTests
{
    private readonly Mock<ICampaniaService> _campaniaServiceMock;
    private readonly Mock<IPedidoService> _pedidoServiceMock;
    private readonly Mock<ILogger<GetCampaniaStatsQueryHandler>> _loggerMock;
    private readonly GetCampaniaStatsQueryHandler _sut;

    public GetCampaniaStatsQueryHandlerTests()
    {
        _campaniaServiceMock = new Mock<ICampaniaService>();
        _pedidoServiceMock = new Mock<IPedidoService>();
        _loggerMock = new Mock<ILogger<GetCampaniaStatsQueryHandler>>();

        _sut = new GetCampaniaStatsQueryHandler(
            _campaniaServiceMock.Object,
            _pedidoServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingCampania_ReturnsStats()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var query = new GetCampaniaStatsQuery { CampaniaId = campaniaId };

        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        campania.FechaFin = DateTime.UtcNow.AddDays(15);

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _pedidoServiceMock
            .Setup(s => s.CountByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(50);

        _pedidoServiceMock
            .Setup(s => s.GetStatsByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((2500m, 50m, 10m, 500m));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.CampaniaId.Should().Be(campaniaId);
        result.Data.TotalBackers.Should().Be(50);
        result.Data.TotalRecaudado.Should().Be(2500m);
        result.Data.PromedioAporte.Should().Be(50m);
        result.Data.AporteMinimo.Should().Be(10m);
        result.Data.AporteMaximo.Should().Be(500m);
        result.Data.DiasRestantes.Should().BeGreaterThanOrEqualTo(14);
    }

    [Fact]
    public async Task Handle_NonExistentCampania_ReturnsNotFound()
    {
        // Arrange
        var query = new GetCampaniaStatsQuery { CampaniaId = Guid.NewGuid() };

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CampaniaCrowdfunding?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Campania);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetCampaniaStatsQuery { CampaniaId = Guid.NewGuid() };

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_CampaniaWithNoEndDate_ReturnsDiasRestantesZero()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var query = new GetCampaniaStatsQuery { CampaniaId = campaniaId };

        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        campania.FechaFin = null;

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _pedidoServiceMock
            .Setup(s => s.CountByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _pedidoServiceMock
            .Setup(s => s.GetStatsByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((0m, 0m, 0m, 0m));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.DiasRestantes.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ExpiredCampania_ReturnsDiasRestantesZero()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var query = new GetCampaniaStatsQuery { CampaniaId = campaniaId };

        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        campania.FechaFin = DateTime.UtcNow.AddDays(-5);

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _pedidoServiceMock
            .Setup(s => s.CountByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        _pedidoServiceMock
            .Setup(s => s.GetStatsByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((500m, 50m, 10m, 100m));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.DiasRestantes.Should().Be(0);
    }
}
