using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Rewards.Commands;

public class ReorderRewardsCommandHandlerTests
{
    private readonly Mock<IRewardService> _rewardServiceMock;
    private readonly Mock<ICampaniaService> _campaniaServiceMock;
    private readonly Mock<IValidator<ReorderRewardsCommand>> _validatorMock;
    private readonly Mock<ILogger<ReorderRewardsCommandHandler>> _loggerMock;
    private readonly ReorderRewardsCommandHandler _sut;

    public ReorderRewardsCommandHandlerTests()
    {
        _rewardServiceMock = new Mock<IRewardService>();
        _campaniaServiceMock = new Mock<ICampaniaService>();
        _validatorMock = new Mock<IValidator<ReorderRewardsCommand>>();
        _loggerMock = new Mock<ILogger<ReorderRewardsCommandHandler>>();
        _sut = new ReorderRewardsCommandHandler(
            _rewardServiceMock.Object,
            _campaniaServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var rewardId1 = Guid.NewGuid();
        var rewardId2 = Guid.NewGuid();

        var command = new ReorderRewardsCommand
        {
            CampaniaId = campaniaId,
            RewardOrders = new List<RewardOrderDto>
            {
                new() { RewardId = rewardId1, Orden = 2 },
                new() { RewardId = rewardId2, Orden = 1 }
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.ExistsAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _rewardServiceMock
            .Setup(s => s.GetByIdAsync(It.Is<CampaniaCrowdfundingRewardId>(id => id.Value == rewardId1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(RewardTestData.CreateValid(rewardId1, campaniaId));

        _rewardServiceMock
            .Setup(s => s.GetByIdAsync(It.Is<CampaniaCrowdfundingRewardId>(id => id.Value == rewardId2), It.IsAny<CancellationToken>()))
            .ReturnsAsync(RewardTestData.CreateValid(rewardId2, campaniaId));

        _rewardServiceMock
            .Setup(s => s.ReorderAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<IEnumerable<(CampaniaCrowdfundingRewardId, int)>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _rewardServiceMock.Verify(s => s.ReorderAsync(
            It.IsAny<CampaniaCrowdfundingId>(),
            It.IsAny<IEnumerable<(CampaniaCrowdfundingRewardId, int)>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = new ReorderRewardsCommand
        {
            CampaniaId = Guid.Empty,
            RewardOrders = new List<RewardOrderDto>()
        };

        var failures = new List<ValidationFailure>
        {
            new("CampaniaId", "El CampaniaId es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
            new("RewardOrders", "La lista de rewards es obligatoria") { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().HaveCount(2);
        _rewardServiceMock.Verify(s => s.ReorderAsync(
            It.IsAny<CampaniaCrowdfundingId>(),
            It.IsAny<IEnumerable<(CampaniaCrowdfundingRewardId, int)>>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CampaniaNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = new ReorderRewardsCommand
        {
            CampaniaId = Guid.NewGuid(),
            RewardOrders = new List<RewardOrderDto>
            {
                new() { RewardId = Guid.NewGuid(), Orden = 1 }
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.ExistsAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Campania);
    }

    [Fact]
    public async Task Handle_RewardNotFound_ReturnsNotFound()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var missingRewardId = Guid.NewGuid();

        var command = new ReorderRewardsCommand
        {
            CampaniaId = campaniaId,
            RewardOrders = new List<RewardOrderDto>
            {
                new() { RewardId = missingRewardId, Orden = 1 }
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.ExistsAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _rewardServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CampaniaCrowdfundingReward?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Reward);
    }

    [Fact]
    public async Task Handle_RewardDoesNotBelongToCampania_ReturnsError()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var otherCampaniaId = Guid.NewGuid();
        var rewardId = Guid.NewGuid();

        var command = new ReorderRewardsCommand
        {
            CampaniaId = campaniaId,
            RewardOrders = new List<RewardOrderDto>
            {
                new() { RewardId = rewardId, Orden = 1 }
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.ExistsAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _rewardServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(RewardTestData.CreateValid(rewardId, otherCampaniaId));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_OperationNotAllowed);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var rewardId = Guid.NewGuid();

        var command = new ReorderRewardsCommand
        {
            CampaniaId = campaniaId,
            RewardOrders = new List<RewardOrderDto>
            {
                new() { RewardId = rewardId, Orden = 1 }
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.ExistsAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
