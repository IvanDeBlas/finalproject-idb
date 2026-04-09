using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Rewards.Commands;

public class CreateRewardCommandHandlerTests
{
    private readonly Mock<IRewardService> _rewardServiceMock;
    private readonly Mock<ICampaniaService> _campaniaServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateRewardCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateRewardCommandHandler>> _loggerMock;
    private readonly CreateRewardCommandHandler _sut;

    public CreateRewardCommandHandlerTests()
    {
        _rewardServiceMock = new Mock<IRewardService>();
        _campaniaServiceMock = new Mock<ICampaniaService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreateRewardCommand>>();
        _loggerMock = new Mock<ILogger<CreateRewardCommandHandler>>();
        _sut = new CreateRewardCommandHandler(
            _rewardServiceMock.Object,
            _campaniaServiceMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private static CreateRewardCommand CreateValidCommand(Guid? campaniaId = null) => new()
    {
        CampaniaId = campaniaId ?? Guid.NewGuid(),
        TipoRewardId = 1,
        Nombre = "Test Reward",
        Descripcion = "Test description",
        ImporteMinimo = 25m,
        MonedaId = 1,
        EsAddOn = false,
        IncluyeEnvioFisico = false,
        Orden = 1
    };

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithRewardDto()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var rewardId = Guid.NewGuid();
        var command = CreateValidCommand(campaniaId);

        var entity = RewardTestData.CreateValid(rewardId, campaniaId);
        var expectedDto = new RewardDto
        {
            Id = rewardId,
            CampaniaId = campaniaId,
            Nombre = "Test Reward",
            ImporteMinimo = 25m,
            EsActivo = true
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.ExistsAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapperMock
            .Setup(m => m.Map<CampaniaCrowdfundingReward>(command))
            .Returns(entity);

        var returnedId = new CampaniaCrowdfundingRewardId(rewardId);
        _rewardServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CampaniaCrowdfundingReward>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnedId);

        _rewardServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(m => m.Map<RewardDto>(entity))
            .Returns(expectedDto);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(rewardId);
        result.Data.Nombre.Should().Be("Test Reward");
        _rewardServiceMock.Verify(s => s.CreateAsync(It.IsAny<CampaniaCrowdfundingReward>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = new CreateRewardCommand
        {
            CampaniaId = Guid.Empty,
            Nombre = "",
            ImporteMinimo = 0
        };

        var failures = new List<ValidationFailure>
        {
            new("CampaniaId", "El CampaniaId es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
            new("Nombre", "El nombre es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().HaveCount(2);
        _rewardServiceMock.Verify(s => s.CreateAsync(It.IsAny<CampaniaCrowdfundingReward>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CampaniaNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = CreateValidCommand();

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
        _rewardServiceMock.Verify(s => s.CreateAsync(It.IsAny<CampaniaCrowdfundingReward>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_OrdenZero_AutoIncrementsOrden()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var rewardId = Guid.NewGuid();
        var command = CreateValidCommand(campaniaId);
        command.Orden = 0;

        var entity = RewardTestData.CreateValid(rewardId, campaniaId);
        entity.Orden = 0;

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.ExistsAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapperMock
            .Setup(m => m.Map<CampaniaCrowdfundingReward>(command))
            .Returns(entity);

        _rewardServiceMock
            .Setup(s => s.GetMaxOrdenAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var returnedId = new CampaniaCrowdfundingRewardId(rewardId);
        _rewardServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CampaniaCrowdfundingReward>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnedId);

        _rewardServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(m => m.Map<RewardDto>(entity))
            .Returns(new RewardDto { Id = rewardId, Orden = 4 });

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        entity.Orden.Should().Be(4);
        _rewardServiceMock.Verify(s => s.GetMaxOrdenAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_OrdenProvided_DoesNotAutoIncrement()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var rewardId = Guid.NewGuid();
        var command = CreateValidCommand(campaniaId);
        command.Orden = 5;

        var entity = RewardTestData.CreateValid(rewardId, campaniaId);
        entity.Orden = 5;

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.ExistsAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapperMock
            .Setup(m => m.Map<CampaniaCrowdfundingReward>(command))
            .Returns(entity);

        var returnedId = new CampaniaCrowdfundingRewardId(rewardId);
        _rewardServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CampaniaCrowdfundingReward>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnedId);

        _rewardServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(m => m.Map<RewardDto>(entity))
            .Returns(new RewardDto { Id = rewardId, Orden = 5 });

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        entity.Orden.Should().Be(5);
        _rewardServiceMock.Verify(s => s.GetMaxOrdenAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = CreateValidCommand();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.ExistsAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapperMock
            .Setup(m => m.Map<CampaniaCrowdfundingReward>(command))
            .Throws(new Exception("Mapping error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
