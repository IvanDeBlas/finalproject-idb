using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Dashboard.Queries;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Dashboard.Queries;

public class GetDashboardResumenQueryHandlerTests
{
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IDashboardService> _dashboardServiceMock;
    private readonly Mock<ICampaniaRepository> _campaniaRepoMock;
    private readonly Mock<IPedidoRepository> _pedidoRepoMock;
    private readonly Mock<ILogger<GetDashboardResumenQueryHandler>> _loggerMock;
    private readonly GetDashboardResumenQueryHandler _sut;

    public GetDashboardResumenQueryHandlerTests()
    {
        _artistaServiceMock = new Mock<IArtistaService>();
        _dashboardServiceMock = new Mock<IDashboardService>();
        _campaniaRepoMock = new Mock<ICampaniaRepository>();
        _pedidoRepoMock = new Mock<IPedidoRepository>();
        _loggerMock = new Mock<ILogger<GetDashboardResumenQueryHandler>>();

        _sut = new GetDashboardResumenQueryHandler(
            _artistaServiceMock.Object,
            _dashboardServiceMock.Object,
            _campaniaRepoMock.Object,
            _pedidoRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidArtist_ReturnsResumen()
    {
        // Arrange
        var query = new GetDashboardResumenQuery { UserId = DashboardTestData.TestUserId };
        var artista = DashboardTestData.CreateArtista();
        var campanias = new List<CampaniaCrowdfunding>
        {
            DashboardTestData.CreateCampania(estadoId: 2, importePledged: 2000m),
            DashboardTestData.CreateCampania(campaniaId: Guid.NewGuid(), estadoId: 3, importePledged: 5000m)
        };

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);

        _dashboardServiceMock
            .Setup(s => s.GetResumenAsync(artista.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((7000m, 100, 1, 1));

        _campaniaRepoMock
            .Setup(r => r.GetByArtistaIdAsync(artista.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(campanias);

        _pedidoRepoMock
            .Setup(r => r.GetLastByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PedidoCrowdfunding?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalRecaudado.Should().Be(7000m);
        result.Data.TotalBackers.Should().Be(100);
        result.Data.CampaniasActivas.Should().Be(1);
        result.Data.CampaniasCompletadas.Should().Be(1);
        result.Data.TotalCampanias.Should().Be(2);
        result.Data.NombreArtistico.Should().Be("Test Artist");
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = new GetDashboardResumenQuery { UserId = "non-existent-user" };

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
        var query = new GetDashboardResumenQuery { UserId = DashboardTestData.TestUserId };

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_NoCampanias_ReturnsEmptyResumen()
    {
        // Arrange
        var query = new GetDashboardResumenQuery { UserId = DashboardTestData.TestUserId };
        var artista = DashboardTestData.CreateArtista();

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);

        _dashboardServiceMock
            .Setup(s => s.GetResumenAsync(artista.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((0m, 0, 0, 0));

        _campaniaRepoMock
            .Setup(r => r.GetByArtistaIdAsync(artista.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CampaniaCrowdfunding>());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.TotalCampanias.Should().Be(0);
        result.Data.TotalRecaudado.Should().Be(0);
        result.Data.FechaUltimoAporte.Should().BeNull();
    }
}
