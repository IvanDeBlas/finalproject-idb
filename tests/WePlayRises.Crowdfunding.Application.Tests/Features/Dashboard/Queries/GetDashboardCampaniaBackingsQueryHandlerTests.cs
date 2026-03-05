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

public class GetDashboardCampaniaBackingsQueryHandlerTests
{
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<ICampaniaService> _campaniaServiceMock;
    private readonly Mock<IPedidoRepository> _pedidoRepoMock;
    private readonly Mock<IDashboardService> _dashboardServiceMock;
    private readonly Mock<IValidator<GetDashboardCampaniaBackingsQuery>> _validatorMock;
    private readonly Mock<ILogger<GetDashboardCampaniaBackingsQueryHandler>> _loggerMock;
    private readonly GetDashboardCampaniaBackingsQueryHandler _sut;

    private readonly Artista _testArtista;
    private readonly CampaniaCrowdfunding _testCampania;

    public GetDashboardCampaniaBackingsQueryHandlerTests()
    {
        _artistaServiceMock = new Mock<IArtistaService>();
        _campaniaServiceMock = new Mock<ICampaniaService>();
        _pedidoRepoMock = new Mock<IPedidoRepository>();
        _dashboardServiceMock = new Mock<IDashboardService>();
        _validatorMock = new Mock<IValidator<GetDashboardCampaniaBackingsQuery>>();
        _loggerMock = new Mock<ILogger<GetDashboardCampaniaBackingsQueryHandler>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetDashboardCampaniaBackingsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _testArtista = DashboardTestData.CreateArtista();
        _testCampania = DashboardTestData.CreateCampania();

        _sut = new GetDashboardCampaniaBackingsQueryHandler(
            _artistaServiceMock.Object,
            _campaniaServiceMock.Object,
            _pedidoRepoMock.Object,
            _dashboardServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsBackingsList()
    {
        // Arrange
        var query = new GetDashboardCampaniaBackingsQuery
        {
            CampaniaId = _testCampania.Id.Value,
            UserId = DashboardTestData.TestUserId,
            Page = 1,
            PageSize = 20
        };

        var pedidos = new List<PedidoCrowdfunding>
        {
            DashboardTestData.CreatePedido(campaniaId: _testCampania.Id.Value, monto: 50m)
        };

        SetupValidOwnership();

        _pedidoRepoMock
            .Setup(r => r.GetByCampaniaIdPaginatedAsync(It.IsAny<CampaniaCrowdfundingId>(), 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedidos);

        _pedidoRepoMock
            .Setup(r => r.CountByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _dashboardServiceMock
            .Setup(s => s.GetBackingPromedioAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(50m);

        _dashboardServiceMock
            .Setup(s => s.GetRewardMasPopularAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("CD Firmado");

        _pedidoRepoMock
            .Setup(r => r.GetLastByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedidos[0]);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Backings.Items.Should().HaveCount(1);
        result.Data.Stats.TotalBackers.Should().Be(1);
        result.Data.Stats.BackingPromedio.Should().Be(50m);
        result.Data.Stats.RewardMasPopular.Should().Be("CD Firmado");
    }

    [Fact]
    public async Task Handle_CampaniaNotOwned_ReturnsForbidden()
    {
        // Arrange
        var otherArtistaId = Guid.NewGuid();
        var campania = DashboardTestData.CreateCampania(artistaId: otherArtistaId);

        var query = new GetDashboardCampaniaBackingsQuery
        {
            CampaniaId = campania.Id.Value,
            UserId = DashboardTestData.TestUserId,
            Page = 1,
            PageSize = 20
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
    public async Task Handle_CampaniaNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = new GetDashboardCampaniaBackingsQuery
        {
            CampaniaId = Guid.NewGuid(),
            UserId = DashboardTestData.TestUserId,
            Page = 1,
            PageSize = 20
        };

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testArtista);

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CampaniaCrowdfunding?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Campania);
    }

    [Fact]
    public async Task Handle_AnonymousBacking_HidesBackerName()
    {
        // Arrange
        var query = new GetDashboardCampaniaBackingsQuery
        {
            CampaniaId = _testCampania.Id.Value,
            UserId = DashboardTestData.TestUserId,
            Page = 1,
            PageSize = 20
        };

        var anonPedido = DashboardTestData.CreatePedido(
            campaniaId: _testCampania.Id.Value, permitirMostrarNombre: false);

        SetupValidOwnership();

        _pedidoRepoMock
            .Setup(r => r.GetByCampaniaIdPaginatedAsync(It.IsAny<CampaniaCrowdfundingId>(), 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PedidoCrowdfunding> { anonPedido });

        _pedidoRepoMock
            .Setup(r => r.CountByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _dashboardServiceMock
            .Setup(s => s.GetBackingPromedioAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(25m);

        _dashboardServiceMock
            .Setup(s => s.GetRewardMasPopularAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        _pedidoRepoMock
            .Setup(r => r.GetLastByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(anonPedido);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var backing = result.Data!.Backings.Items.First();
        backing.NombreBacker.Should().Be("Anonimo");
        backing.EsAnonimo.Should().BeTrue();
        backing.Email.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetDashboardCampaniaBackingsQuery
        {
            CampaniaId = _testCampania.Id.Value,
            UserId = DashboardTestData.TestUserId,
            Page = 1,
            PageSize = 20
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
