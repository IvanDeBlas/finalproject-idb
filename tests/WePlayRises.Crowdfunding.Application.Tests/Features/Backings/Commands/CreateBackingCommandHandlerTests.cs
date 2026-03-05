using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Backings.Commands;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Backings.Commands;

public class CreateBackingCommandHandlerTests
{
    private readonly Mock<IBackingService> _backingServiceMock;
    private readonly Mock<ICampaniaService> _campaniaServiceMock;
    private readonly Mock<IRewardService> _rewardServiceMock;
    private readonly Mock<IPedidoService> _pedidoServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateBackingCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateBackingCommandHandler>> _loggerMock;
    private readonly CreateBackingCommandHandler _sut;

    public CreateBackingCommandHandlerTests()
    {
        _backingServiceMock = new Mock<IBackingService>();
        _campaniaServiceMock = new Mock<ICampaniaService>();
        _rewardServiceMock = new Mock<IRewardService>();
        _pedidoServiceMock = new Mock<IPedidoService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreateBackingCommand>>();
        _loggerMock = new Mock<ILogger<CreateBackingCommandHandler>>();

        _sut = new CreateBackingCommandHandler(
            _backingServiceMock.Object,
            _campaniaServiceMock.Object,
            _rewardServiceMock.Object,
            _pedidoServiceMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithBackingDto()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var pedidoId = Guid.NewGuid();
        var command = BackingTestData.CreateValidCommand(campaniaId: campaniaId);

        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        var pedido = BackingTestData.CreateCompletedPedido(pedidoId: pedidoId, campaniaId: campaniaId, monto: command.Monto);
        var expectedDto = new BackingDto
        {
            Id = pedidoId,
            CampaniaId = campaniaId,
            Monto = command.Monto,
            EsAnonimo = false,
            FechaCreacion = DateTime.UtcNow
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        var returnedPedidoId = new PedidoCrowdfundingId(pedidoId);
        _backingServiceMock
            .Setup(s => s.CreateBackingAsync(
                It.IsAny<CampaniaCrowdfundingId>(),
                It.IsAny<CampaniaCrowdfundingRewardId?>(),
                It.IsAny<decimal>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnedPedidoId);

        _pedidoServiceMock
            .Setup(s => s.GetByIdAsync(returnedPedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        _mapperMock
            .Setup(m => m.Map<BackingDto>(pedido))
            .Returns(expectedDto);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(pedidoId);
        result.Data.CampaniaTitulo.Should().Be(campania.Titulo);
        result.Data.MonedaSimbolo.Should().Be("EUR");
        result.Data.EstadoPedido.Should().Be("Completado");
        _backingServiceMock.Verify(s => s.CreateBackingAsync(
            It.IsAny<CampaniaCrowdfundingId>(),
            It.IsAny<CampaniaCrowdfundingRewardId?>(),
            command.Monto,
            command.UserId,
            command.Mensaje,
            command.EsAnonimo,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommandWithReward_ResolvesRewardName()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var rewardId = Guid.NewGuid();
        var pedidoId = Guid.NewGuid();
        var command = BackingTestData.CreateValidCommand(campaniaId: campaniaId, rewardId: rewardId, monto: 50m);

        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        var reward = BackingTestData.CreateActiveReward(rewardId: rewardId, campaniaId: campaniaId, importeMinimo: 25m);
        var pedido = BackingTestData.CreateCompletedPedido(pedidoId: pedidoId, campaniaId: campaniaId, monto: 50m);
        var expectedDto = new BackingDto { Id = pedidoId, CampaniaId = campaniaId, Monto = 50m };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _rewardServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingRewardId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reward);

        _backingServiceMock
            .Setup(s => s.CreateBackingAsync(
                It.IsAny<CampaniaCrowdfundingId>(),
                It.IsAny<CampaniaCrowdfundingRewardId?>(),
                It.IsAny<decimal>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PedidoCrowdfundingId(pedidoId));

        _pedidoServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PedidoCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        _mapperMock
            .Setup(m => m.Map<BackingDto>(pedido))
            .Returns(expectedDto);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.RewardNombre.Should().Be(reward.Nombre);
    }

    [Fact]
    public async Task Handle_AnonymousBacking_SetsUserNameToAnonimo()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var pedidoId = Guid.NewGuid();
        var command = BackingTestData.CreateValidCommand(campaniaId: campaniaId);
        command.EsAnonimo = true;

        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        var pedido = BackingTestData.CreateCompletedPedido(pedidoId: pedidoId, campaniaId: campaniaId);
        var expectedDto = new BackingDto { Id = pedidoId, CampaniaId = campaniaId, Monto = 25m };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _backingServiceMock
            .Setup(s => s.CreateBackingAsync(
                It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CampaniaCrowdfundingRewardId?>(),
                It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PedidoCrowdfundingId(pedidoId));

        _pedidoServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PedidoCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        _mapperMock.Setup(m => m.Map<BackingDto>(pedido)).Returns(expectedDto);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.UserName.Should().Be("Anonimo");
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = BackingTestData.CreateValidCommand();
        command.CampaniaId = Guid.Empty;
        command.Monto = 0;

        var failures = new List<ValidationFailure>
        {
            new("CampaniaId", "El ID de campania es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
            new("Monto", "El monto debe ser al menos 1 EUR") { ErrorCode = ServiceResponseMessageType.Validation_InvalidAmount }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().HaveCount(2);
        _backingServiceMock.Verify(s => s.CreateBackingAsync(
            It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CampaniaCrowdfundingRewardId?>(),
            It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_BackingServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = BackingTestData.CreateValidCommand();
        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: command.CampaniaId);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _backingServiceMock
            .Setup(s => s.CreateBackingAsync(
                It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CampaniaCrowdfundingRewardId?>(),
                It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_SuccessfulBacking_ReturnsCreatedHttpStatus()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var pedidoId = Guid.NewGuid();
        var command = BackingTestData.CreateValidCommand(campaniaId: campaniaId);
        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        var pedido = BackingTestData.CreateCompletedPedido(pedidoId: pedidoId, campaniaId: campaniaId);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _campaniaServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>())).ReturnsAsync(campania);
        _backingServiceMock.Setup(s => s.CreateBackingAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CampaniaCrowdfundingRewardId?>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(new PedidoCrowdfundingId(pedidoId));
        _pedidoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<PedidoCrowdfundingId>(), It.IsAny<CancellationToken>())).ReturnsAsync(pedido);
        _mapperMock.Setup(m => m.Map<BackingDto>(pedido)).Returns(new BackingDto { Id = pedidoId });

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Messages.Should().ContainSingle(m => m.HttpStatusCode == System.Net.HttpStatusCode.Created);
    }
}
