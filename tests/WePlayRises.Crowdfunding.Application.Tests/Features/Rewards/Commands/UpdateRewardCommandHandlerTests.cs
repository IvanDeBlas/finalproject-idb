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

public class UpdateRewardCommandHandlerTests
{
    private readonly Mock<IRewardService> _serviceMock;
    private readonly Mock<IValidator<UpdateRewardCommand>> _validatorMock;
    private readonly Mock<ILogger<UpdateRewardCommandHandler>> _loggerMock;
    private readonly UpdateRewardCommandHandler _sut;

    public UpdateRewardCommandHandlerTests()
    {
        _serviceMock = new Mock<IRewardService>();
        _validatorMock = new Mock<IValidator<UpdateRewardCommand>>();
        _loggerMock = new Mock<ILogger<UpdateRewardCommandHandler>>();
        _sut = new UpdateRewardCommandHandler(
            _serviceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesReward()
    {
        // Arrange
        var rewardId = Guid.NewGuid();
        var entity = RewardTestData.CreateValid(rewardId);
        var command = new UpdateRewardCommand
        {
            Id = rewardId,
            Nombre = "Updated Name"
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

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
        entity.Nombre.Should().Be("Updated Name");
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfundingReward>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsErrors()
    {
        // Arrange
        var command = new UpdateRewardCommand { Id = Guid.Empty };

        var failures = new List<ValidationFailure>
        {
            new("Id", "El Id es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().HaveCount(1);
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfundingReward>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RewardNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = new UpdateRewardCommand { Id = Guid.NewGuid(), Nombre = "Updated" };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

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
    public async Task Handle_RewardHasBackings_BlocksCriticalFieldChanges()
    {
        // Arrange
        var rewardId = Guid.NewGuid();
        var entity = RewardTestData.CreateValid(rewardId);
        var command = new UpdateRewardCommand
        {
            Id = rewardId,
            ImporteMinimo = 50m
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

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
    public async Task Handle_RewardHasBackings_AllowsDescriptionChange()
    {
        // Arrange
        var rewardId = Guid.NewGuid();
        var entity = RewardTestData.CreateValid(rewardId);
        var command = new UpdateRewardCommand
        {
            Id = rewardId,
            Descripcion = "Updated description"
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _serviceMock
            .Setup(s => s.HasBackingsAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfundingReward>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        entity.Descripcion.Should().Be("Updated description");
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = new UpdateRewardCommand { Id = Guid.NewGuid(), Nombre = "Updated" };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

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
