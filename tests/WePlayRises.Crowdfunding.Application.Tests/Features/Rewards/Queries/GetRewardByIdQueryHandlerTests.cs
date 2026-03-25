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

public class GetRewardByIdQueryHandlerTests
{
    private readonly Mock<IRewardService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetRewardByIdQueryHandler>> _loggerMock;
    private readonly GetRewardByIdQueryHandler _sut;

    public GetRewardByIdQueryHandlerTests()
    {
        _serviceMock = new Mock<IRewardService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetRewardByIdQueryHandler>>();
        _sut = new GetRewardByIdQueryHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingReward_ReturnsRewardDto()
    {
        // Arrange
        var rewardId = Guid.NewGuid();
        var query = new GetRewardByIdQuery { Id = rewardId };
        var entity = RewardTestData.CreateValid(rewardId: rewardId);
        var expectedDto = new RewardDto
        {
            Id = rewardId,
            Nombre = entity.Nombre,
            ImporteMinimo = entity.ImporteMinimo
        };

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(m => m.Map<RewardDto>(entity))
            .Returns(expectedDto);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(rewardId);
        result.Data.Nombre.Should().Be(entity.Nombre);
        result.Data.ImporteMinimo.Should().Be(entity.ImporteMinimo);
    }

    [Fact]
    public async Task Handle_NonExistentReward_ReturnsNotFound()
    {
        // Arrange
        var query = new GetRewardByIdQuery { Id = Guid.NewGuid() };

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CampaniaCrowdfundingReward?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Reward);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetRewardByIdQuery { Id = Guid.NewGuid() };

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
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
