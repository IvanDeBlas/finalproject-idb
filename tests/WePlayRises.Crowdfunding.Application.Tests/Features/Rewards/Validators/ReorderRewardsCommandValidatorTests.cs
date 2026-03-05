using FluentAssertions;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Validators;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Rewards.Validators;

public class ReorderRewardsCommandValidatorTests
{
    private readonly ReorderRewardsCommandValidator _sut;

    public ReorderRewardsCommandValidatorTests()
    {
        _sut = new ReorderRewardsCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_IsValid()
    {
        // Arrange
        var command = new ReorderRewardsCommand
        {
            CampaniaId = Guid.NewGuid(),
            RewardOrders = new List<RewardOrderDto>
            {
                new() { RewardId = Guid.NewGuid(), Orden = 1 },
                new() { RewardId = Guid.NewGuid(), Orden = 2 }
            }
        };

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyCampaniaId_HasError()
    {
        // Arrange
        var command = new ReorderRewardsCommand
        {
            CampaniaId = Guid.Empty,
            RewardOrders = new List<RewardOrderDto>
            {
                new() { RewardId = Guid.NewGuid(), Orden = 1 }
            }
        };

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "CampaniaId" &&
            e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_NullRewardOrders_HasError()
    {
        // Arrange
        var command = new ReorderRewardsCommand
        {
            CampaniaId = Guid.NewGuid(),
            RewardOrders = null!
        };

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "RewardOrders" &&
            e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyRewardOrders_HasError()
    {
        // Arrange
        var command = new ReorderRewardsCommand
        {
            CampaniaId = Guid.NewGuid(),
            RewardOrders = new List<RewardOrderDto>()
        };

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "RewardOrders" &&
            e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyRewardId_HasError()
    {
        // Arrange
        var command = new ReorderRewardsCommand
        {
            CampaniaId = Guid.NewGuid(),
            RewardOrders = new List<RewardOrderDto>
            {
                new() { RewardId = Guid.Empty, Orden = 1 }
            }
        };

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_Required &&
            e.ErrorMessage.Contains("RewardId"));
    }

    [Fact]
    public async Task Validate_NegativeOrden_HasError()
    {
        // Arrange
        var command = new ReorderRewardsCommand
        {
            CampaniaId = Guid.NewGuid(),
            RewardOrders = new List<RewardOrderDto>
            {
                new() { RewardId = Guid.NewGuid(), Orden = -1 }
            }
        };

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_InvalidRange);
    }
}
