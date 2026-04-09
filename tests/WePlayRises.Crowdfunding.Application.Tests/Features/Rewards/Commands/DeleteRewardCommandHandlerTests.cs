using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Rewards.Commands;

public class DeleteRewardCommandHandlerTests
{
    private readonly Mock<IRewardService> _serviceMock;
    private readonly Mock<ILogger<DeleteRewardCommandHandler>> _loggerMock;
    private readonly DeleteRewardCommandHandler _sut;

    public DeleteRewardCommandHandlerTests()
    {
        _serviceMock = new Mock<IRewardService>();
        _loggerMock = new Mock<ILogger<DeleteRewardCommandHandler>>();
        _sut = new DeleteRewardCommandHandler(
            _serviceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidId_SoftDeletesReward()
    {
        // Arrange
        var rewardId = Guid.NewGuid();
        var entity = RewardTestData.CreateValid(rewardId);
        var command = new DeleteRewardCommand(rewardId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _serviceMock
            .Setup(s => s.HasBackingsAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfundingReward>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        entity.EsActivo.Should().BeFalse();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<CampaniaCrowdfundingReward>(r => !r.EsActivo),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RewardNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = new DeleteRewardCommand(Guid.NewGuid());

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CampaniaCrowdfundingReward?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Reward);
    }

    [Fact]
    public async Task Handle_RewardHasBackings_ReturnsConflict()
    {
        // Arrange
        var rewardId = Guid.NewGuid();
        var entity = RewardTestData.CreateValid(rewardId);
        var command = new DeleteRewardCommand(rewardId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _serviceMock
            .Setup(s => s.HasBackingsAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_RewardHasBackings);
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfundingReward>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = new DeleteRewardCommand(Guid.NewGuid());

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
