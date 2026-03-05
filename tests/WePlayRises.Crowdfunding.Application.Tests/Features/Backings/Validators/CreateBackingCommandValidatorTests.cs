using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Features.Backings.Commands;
using WePlayRises.Crowdfunding.Application.Features.Backings.Validators;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Backings.Validators;

public class CreateBackingCommandValidatorTests
{
    private readonly Mock<ICampaniaService> _campaniaServiceMock;
    private readonly Mock<IRewardService> _rewardServiceMock;
    private readonly CreateBackingCommandValidator _sut;

    public CreateBackingCommandValidatorTests()
    {
        _campaniaServiceMock = new Mock<ICampaniaService>();
        _rewardServiceMock = new Mock<IRewardService>();
        _sut = new CreateBackingCommandValidator(
            _campaniaServiceMock.Object,
            _rewardServiceMock.Object);
    }

    private CreateBackingCommand CreateValidCommand()
    {
        var campaniaId = Guid.NewGuid();
        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        return BackingTestData.CreateValidCommand(campaniaId: campaniaId);
    }

    [Fact]
    public async Task Validate_ValidCommand_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyCampaniaId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.CampaniaId = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.CampaniaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_MontoZero_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Monto = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Monto)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Validate_MontoNegative_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Monto = -5m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Monto)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Validate_MensajeTooLong_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Mensaje = new string('A', 501);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Mensaje)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_CampaniaNotFound_ReturnsError()
    {
        // Arrange
        var command = BackingTestData.CreateValidCommand();

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CampaniaCrowdfunding?)null);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.NotFound_Campania);
    }

    [Fact]
    public async Task Validate_CampaniaNotPublished_ReturnsError()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        campania.EstadoCampaniaId = 1; // Borrador

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        var command = BackingTestData.CreateValidCommand(campaniaId: campaniaId);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.BusinessRule_CampaniaNotActive);
    }

    [Fact]
    public async Task Validate_CampaniaEnded_ReturnsError()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        campania.FechaFin = DateTime.UtcNow.AddDays(-1); // Expired

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        var command = BackingTestData.CreateValidCommand(campaniaId: campaniaId);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.BusinessRule_CampaniaEnded);
    }

    [Fact]
    public async Task Validate_AnonymousUserWhenNotAllowed_ReturnsError()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId, permiteAnonimas: false);

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        var command = BackingTestData.CreateValidCommand(campaniaId: campaniaId, userId: null);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Auth_UserNotAuthenticated);
    }

    [Fact]
    public async Task Validate_AnonymousUserWhenAllowed_ReturnsValid()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId, permiteAnonimas: true);

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        var command = BackingTestData.CreateValidCommand(campaniaId: campaniaId, userId: null);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_RewardNotFound_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.RewardId = Guid.NewGuid();

        _rewardServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CampaniaCrowdfundingReward?)null);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.NotFound_Reward);
    }

    [Fact]
    public async Task Validate_RewardOutOfStock_ReturnsError()
    {
        // Arrange
        var rewardId = Guid.NewGuid();
        var command = CreateValidCommand();
        command.RewardId = rewardId;

        var reward = BackingTestData.CreateActiveReward(rewardId: rewardId, cantidadMaxima: 10);

        _rewardServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reward);

        _rewardServiceMock
            .Setup(s => s.HasStockAvailableAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.BusinessRule_RewardOutOfStock);
    }

    [Fact]
    public async Task Validate_MontoBelowRewardMinimum_ReturnsError()
    {
        // Arrange
        var rewardId = Guid.NewGuid();
        var command = CreateValidCommand();
        command.RewardId = rewardId;
        command.Monto = 5m; // Below minimum of 25

        var reward = BackingTestData.CreateActiveReward(rewardId: rewardId, importeMinimo: 25m);

        _rewardServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reward);

        _rewardServiceMock
            .Setup(s => s.HasStockAvailableAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.BusinessRule_AmountBelowMinimum);
    }

    [Fact]
    public async Task Validate_ValidCommandWithReward_ReturnsValid()
    {
        // Arrange
        var rewardId = Guid.NewGuid();
        var command = CreateValidCommand();
        command.RewardId = rewardId;
        command.Monto = 50m;

        var reward = BackingTestData.CreateActiveReward(rewardId: rewardId, importeMinimo: 25m);

        _rewardServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reward);

        _rewardServiceMock
            .Setup(s => s.HasStockAvailableAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
