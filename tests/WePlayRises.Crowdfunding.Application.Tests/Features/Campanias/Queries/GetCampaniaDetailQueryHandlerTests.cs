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

public class GetCampaniaDetailQueryHandlerTests
{
    private readonly Mock<ICampaniaService> _campaniaServiceMock;
    private readonly Mock<IRewardService> _rewardServiceMock;
    private readonly Mock<ICrowdFlagsService> _crowdFlagsServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetCampaniaDetailQueryHandler>> _loggerMock;
    private readonly GetCampaniaDetailQueryHandler _sut;

    public GetCampaniaDetailQueryHandlerTests()
    {
        _campaniaServiceMock = new Mock<ICampaniaService>();
        _rewardServiceMock = new Mock<IRewardService>();
        _crowdFlagsServiceMock = new Mock<ICrowdFlagsService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetCampaniaDetailQueryHandler>>();

        _crowdFlagsServiceMock
            .Setup(x => x.GetFlagsForProyectosAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, CrowdFlags>());

        _sut = new GetCampaniaDetailQueryHandler(
            _campaniaServiceMock.Object,
            _rewardServiceMock.Object,
            _crowdFlagsServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingCampania_ReturnsDetailDto()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var query = new GetCampaniaDetailQuery { Id = campaniaId };

        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        campania.ImportePledgedActual = 2500m;
        campania.ImporteObjetivo = 5000m;

        var expectedDto = new CampaniaDetailDto
        {
            Id = campaniaId,
            Titulo = campania.Titulo,
            ImporteObjetivo = 5000m,
            ImportePledgedActual = 2500m
        };

        _campaniaServiceMock
            .Setup(s => s.GetDetailByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _campaniaServiceMock
            .Setup(s => s.GetTotalBackersAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        _mapperMock
            .Setup(m => m.Map<CampaniaDetailDto>(campania))
            .Returns(expectedDto);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.PorcentajeProgreso.Should().Be(50.0m);
        result.Data.MonedaSimbolo.Should().Be("EUR");
        result.Data.TotalBackers.Should().Be(10);
    }

    [Fact]
    public async Task Handle_NonExistentCampania_ReturnsNotFound()
    {
        // Arrange
        var query = new GetCampaniaDetailQuery { Id = Guid.NewGuid() };

        _campaniaServiceMock
            .Setup(s => s.GetDetailByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
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
        var query = new GetCampaniaDetailQuery { Id = Guid.NewGuid() };

        _campaniaServiceMock
            .Setup(s => s.GetDetailByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_CampaniaWithRewards_MapsRewardsWithStock()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var rewardId = Guid.NewGuid();
        var query = new GetCampaniaDetailQuery { Id = campaniaId };

        var reward = BackingTestData.CreateActiveReward(rewardId: rewardId, campaniaId: campaniaId, cantidadMaxima: 100);
        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        campania.Rewards = new List<CampaniaCrowdfundingReward> { reward };

        var rewardDto = new RewardPublicDto { Id = rewardId, Nombre = reward.Nombre, ImporteMinimo = reward.ImporteMinimo };

        _campaniaServiceMock
            .Setup(s => s.GetDetailByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _campaniaServiceMock
            .Setup(s => s.GetTotalBackersAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        _mapperMock
            .Setup(m => m.Map<CampaniaDetailDto>(campania))
            .Returns(new CampaniaDetailDto { Id = campaniaId, ImporteObjetivo = 5000m });

        _mapperMock
            .Setup(m => m.Map<RewardPublicDto>(reward))
            .Returns(rewardDto);

        _rewardServiceMock
            .Setup(s => s.GetCantidadVendidaAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(25);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Rewards.Should().HaveCount(1);
        result.Data.Rewards[0].CantidadVendida.Should().Be(25);
        result.Data.Rewards[0].Disponible.Should().BeTrue();
    }
}
