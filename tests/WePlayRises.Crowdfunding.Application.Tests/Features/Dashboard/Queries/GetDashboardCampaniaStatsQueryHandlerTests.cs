using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
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

public class GetDashboardCampaniaStatsQueryHandlerTests
{
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<ICampaniaService> _campaniaServiceMock;
    private readonly Mock<IPedidoRepository> _pedidoRepoMock;
    private readonly Mock<IDashboardService> _dashboardServiceMock;
    private readonly Mock<IRewardRepository> _rewardRepoMock;
    private readonly Mock<IValidator<GetDashboardCampaniaStatsQuery>> _validatorMock;
    private readonly Mock<ILogger<GetDashboardCampaniaStatsQueryHandler>> _loggerMock;
    private readonly GetDashboardCampaniaStatsQueryHandler _sut;

    private readonly Artista _testArtista;
    private readonly CampaniaCrowdfunding _testCampania;

    public GetDashboardCampaniaStatsQueryHandlerTests()
    {
        _artistaServiceMock = new Mock<IArtistaService>();
        _campaniaServiceMock = new Mock<ICampaniaService>();
        _pedidoRepoMock = new Mock<IPedidoRepository>();
        _dashboardServiceMock = new Mock<IDashboardService>();
        _rewardRepoMock = new Mock<IRewardRepository>();
        _validatorMock = new Mock<IValidator<GetDashboardCampaniaStatsQuery>>();
        _loggerMock = new Mock<ILogger<GetDashboardCampaniaStatsQueryHandler>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetDashboardCampaniaStatsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _testArtista = DashboardTestData.CreateArtista();
        _testCampania = DashboardTestData.CreateCampania();

        _sut = new GetDashboardCampaniaStatsQueryHandler(
            _artistaServiceMock.Object,
            _campaniaServiceMock.Object,
            _pedidoRepoMock.Object,
            _dashboardServiceMock.Object,
            _rewardRepoMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsStats()
    {
        // Arrange
        var query = new GetDashboardCampaniaStatsQuery
        {
            CampaniaId = _testCampania.Id.Value,
            UserId = DashboardTestData.TestUserId
        };

        SetupValidOwnership();

        _pedidoRepoMock
            .Setup(r => r.CountByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        _dashboardServiceMock
            .Setup(s => s.GetBackingPromedioAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(100m);

        _dashboardServiceMock
            .Setup(s => s.GetVelocidadDiariaAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(50m);

        _dashboardServiceMock
            .Setup(s => s.CalcularProyeccionFinalAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(6000m);

        _pedidoRepoMock
            .Setup(r => r.GetRewardStatsByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, int>());

        _rewardRepoMock
            .Setup(r => r.GetByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CampaniaCrowdfundingReward>());

        _pedidoRepoMock
            .Setup(r => r.GetProgressByDayAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<(DateTime, int, decimal)>
            {
                (DateTime.UtcNow.AddDays(-2), 3, 300m),
                (DateTime.UtcNow.AddDays(-1), 5, 500m),
                (DateTime.UtcNow, 2, 200m)
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.NumBackers.Should().Be(10);
        result.Data.BackingPromedio.Should().Be(100m);
        result.Data.VelocidadDiaria.Should().Be(50m);
        result.Data.ProyeccionFinal.Should().Be(6000m);
        result.Data.ProgressoPorDia.Should().HaveCount(3);
        result.Data.ProgressoPorDia[2].Acumulado.Should().Be(1000m);
    }

    [Fact]
    public async Task Handle_CampaniaNotOwned_ReturnsForbidden()
    {
        // Arrange
        var otherArtistaId = Guid.NewGuid();
        var campania = DashboardTestData.CreateCampania(artistaId: otherArtistaId);

        var query = new GetDashboardCampaniaStatsQuery
        {
            CampaniaId = campania.Id.Value,
            UserId = DashboardTestData.TestUserId
        };

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testArtista);

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = new GetDashboardCampaniaStatsQuery
        {
            CampaniaId = _testCampania.Id.Value,
            UserId = "non-existent"
        };

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
        var query = new GetDashboardCampaniaStatsQuery
        {
            CampaniaId = _testCampania.Id.Value,
            UserId = DashboardTestData.TestUserId
        };

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

    private void SetupValidOwnership()
    {
        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testArtista);

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testCampania);
    }
}
