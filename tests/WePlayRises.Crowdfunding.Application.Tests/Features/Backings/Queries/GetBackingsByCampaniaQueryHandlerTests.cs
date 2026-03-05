using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Backings.Queries;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Backings.Queries;

public class GetBackingsByCampaniaQueryHandlerTests
{
    private readonly Mock<ICampaniaService> _campaniaServiceMock;
    private readonly Mock<IPedidoService> _pedidoServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<GetBackingsByCampaniaQuery>> _validatorMock;
    private readonly Mock<ILogger<GetBackingsByCampaniaQueryHandler>> _loggerMock;
    private readonly GetBackingsByCampaniaQueryHandler _sut;

    public GetBackingsByCampaniaQueryHandlerTests()
    {
        _campaniaServiceMock = new Mock<ICampaniaService>();
        _pedidoServiceMock = new Mock<IPedidoService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<GetBackingsByCampaniaQuery>>();
        _loggerMock = new Mock<ILogger<GetBackingsByCampaniaQueryHandler>>();

        _sut = new GetBackingsByCampaniaQueryHandler(
            _campaniaServiceMock.Object,
            _pedidoServiceMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsBackingsList()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var query = new GetBackingsByCampaniaQuery { CampaniaId = campaniaId, Limit = 10 };

        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        var pedido = BackingTestData.CreateCompletedPedido(campaniaId: campaniaId);
        pedido.PermitirMostrarNombre = true;
        pedido.UserId = "user-123";
        pedido.Lineas = new List<PedidoCrowdfundingLinea>();

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _pedidoServiceMock
            .Setup(s => s.GetRecentByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PedidoCrowdfunding> { pedido });

        _mapperMock
            .Setup(m => m.Map<BackingPublicDto>(pedido))
            .Returns(new BackingPublicDto { Id = pedido.Id.Value, Monto = pedido.ImporteTotal });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data![0].NombreBacker.Should().Be("Backer");
    }

    [Fact]
    public async Task Handle_AnonymousBacking_SetsNombreAsAnonimo()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var query = new GetBackingsByCampaniaQuery { CampaniaId = campaniaId };

        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        var pedido = BackingTestData.CreateCompletedPedido(campaniaId: campaniaId);
        pedido.PermitirMostrarNombre = false;
        pedido.UserId = "user-123";
        pedido.Lineas = new List<PedidoCrowdfundingLinea>();

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _pedidoServiceMock
            .Setup(s => s.GetRecentByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PedidoCrowdfunding> { pedido });

        _mapperMock
            .Setup(m => m.Map<BackingPublicDto>(pedido))
            .Returns(new BackingPublicDto { Id = pedido.Id.Value, Monto = pedido.ImporteTotal });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data![0].NombreBacker.Should().Be("Anonimo");
    }

    [Fact]
    public async Task Handle_BackingWithReward_ResolvesRewardName()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var rewardId = Guid.NewGuid();
        var query = new GetBackingsByCampaniaQuery { CampaniaId = campaniaId };

        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);
        var reward = BackingTestData.CreateActiveReward(rewardId: rewardId, campaniaId: campaniaId);
        var linea = new PedidoCrowdfundingLinea
        {
            RewardId = new CampaniaCrowdfundingRewardId(rewardId),
            Reward = reward,
            Cantidad = 1,
            PrecioUnitario = 25m,
            ImporteLinea = 25m,
            EsRewardPrincipal = true,
            FechaCreacion = DateTime.UtcNow
        };

        var pedido = BackingTestData.CreateCompletedPedido(campaniaId: campaniaId);
        pedido.PermitirMostrarNombre = true;
        pedido.UserId = "user-123";
        pedido.Lineas = new List<PedidoCrowdfundingLinea> { linea };

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _pedidoServiceMock
            .Setup(s => s.GetRecentByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PedidoCrowdfunding> { pedido });

        _mapperMock
            .Setup(m => m.Map<BackingPublicDto>(pedido))
            .Returns(new BackingPublicDto { Id = pedido.Id.Value, Monto = pedido.ImporteTotal });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data![0].RewardNombre.Should().Be("CD Firmado");
    }

    [Fact]
    public async Task Handle_NonExistentCampania_ReturnsNotFound()
    {
        // Arrange
        var query = new GetBackingsByCampaniaQuery { CampaniaId = Guid.NewGuid() };

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
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
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var query = new GetBackingsByCampaniaQuery { CampaniaId = Guid.Empty, Limit = 0 };

        var failures = new List<ValidationFailure>
        {
            new("CampaniaId", "El ID de campania es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
            new("Limit", "El limite debe estar entre 1 y 100") { ErrorCode = ServiceResponseMessageType.Validation_InvalidRange }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetBackingsByCampaniaQuery { CampaniaId = Guid.NewGuid() };

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_EmptyBackings_ReturnsEmptyList()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var query = new GetBackingsByCampaniaQuery { CampaniaId = campaniaId };

        var campania = BackingTestData.CreatePublicadaCampania(campaniaId: campaniaId);

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _campaniaServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(campania);

        _pedidoServiceMock
            .Setup(s => s.GetRecentByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PedidoCrowdfunding>());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }
}
