using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Queries;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Rewards.Queries;

public class GetAllRewardsQueryHandlerTests
{
    private readonly Mock<IRewardService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetAllRewardsQueryHandler>> _loggerMock;
    private readonly GetAllRewardsQueryHandler _sut;

    public GetAllRewardsQueryHandlerTests()
    {
        _serviceMock = new Mock<IRewardService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetAllRewardsQueryHandler>>();
        _sut = new GetAllRewardsQueryHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllRewards()
    {
        // Arrange
        var entities = new List<CampaniaCrowdfundingReward>
        {
            RewardTestData.CreateValid(orden: 1),
            RewardTestData.CreateValid(orden: 2)
        };

        var expectedDtos = entities.Select(e => new RewardListDto
        {
            Id = e.Id.Value,
            Nombre = e.Nombre,
            Orden = e.Orden
        }).ToList();

        var query = new GetAllRewardsQuery();

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<RewardListDto>>(It.IsAny<IEnumerable<CampaniaCrowdfundingReward>>()))
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
    public async Task Handle_FilterByCampaniaId_UsesGetByCampaniaId()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var entity = RewardTestData.CreateValid(campaniaId: campaniaId);
        var entities = new List<CampaniaCrowdfundingReward> { entity };

        var query = new GetAllRewardsQuery { CampaniaId = campaniaId };

        _serviceMock
            .Setup(s => s.GetByCampaniaIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<RewardListDto>>(It.IsAny<IEnumerable<CampaniaCrowdfundingReward>>()))
            .Returns(new List<RewardListDto>
            {
                new() { Id = entity.Id.Value, CampaniaId = campaniaId }
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        _serviceMock.Verify(s => s.GetByCampaniaIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()), Times.Once);
        _serviceMock.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_FilterByEsActivo_ReturnsOnlyActiveRewards()
    {
        // Arrange
        var active = RewardTestData.CreateValid(esActivo: true);
        var inactive = RewardTestData.CreateValid(esActivo: false);
        var entities = new List<CampaniaCrowdfundingReward> { active, inactive };

        var query = new GetAllRewardsQuery { EsActivo = true };

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<RewardListDto>>(It.Is<IEnumerable<CampaniaCrowdfundingReward>>(
                list => list.All(r => r.EsActivo))))
            .Returns(new List<RewardListDto>
            {
                new() { Id = active.Id.Value, EsActivo = true }
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_FilterByEsAddOn_ReturnsOnlyAddOns()
    {
        // Arrange
        var regular = RewardTestData.CreateValid();
        regular.EsAddOn = false;
        var addOn = RewardTestData.CreateValid();
        addOn.EsAddOn = true;
        var entities = new List<CampaniaCrowdfundingReward> { regular, addOn };

        var query = new GetAllRewardsQuery { EsAddOn = true };

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<RewardListDto>>(It.Is<IEnumerable<CampaniaCrowdfundingReward>>(
                list => list.All(r => r.EsAddOn))))
            .Returns(new List<RewardListDto>
            {
                new() { Id = addOn.Id.Value, EsAddOn = true }
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllRewardsQuery();

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CampaniaCrowdfundingReward>());

        _mapperMock
            .Setup(m => m.Map<IEnumerable<RewardListDto>>(It.IsAny<IEnumerable<CampaniaCrowdfundingReward>>()))
            .Returns(new List<RewardListDto>());

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
        var query = new GetAllRewardsQuery();

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
