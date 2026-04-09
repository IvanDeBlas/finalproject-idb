using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Pedidos.Queries;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Pedidos.Queries;

public class GetAllPedidosQueryHandlerTests
{
    private readonly Mock<IPedidoService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetAllPedidosQueryHandler>> _loggerMock;
    private readonly GetAllPedidosQueryHandler _sut;

    public GetAllPedidosQueryHandlerTests()
    {
        _serviceMock = new Mock<IPedidoService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetAllPedidosQueryHandler>>();
        _sut = new GetAllPedidosQueryHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllPedidos()
    {
        // Arrange
        var entities = new List<PedidoCrowdfunding>
        {
            PedidoTestData.CreateValid(),
            PedidoTestData.CreateValid()
        };

        var expectedDtos = entities.Select(e => new PedidoListDto
        {
            Id = e.Id.Value,
            ImporteTotal = e.ImporteTotal
        }).ToList();

        var query = new GetAllPedidosQuery();

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<PedidoListDto>>(It.IsAny<IEnumerable<PedidoCrowdfunding>>()))
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
        var entity = PedidoTestData.CreateValid(campaniaId: campaniaId);
        var entities = new List<PedidoCrowdfunding> { entity };

        var query = new GetAllPedidosQuery { CampaniaId = campaniaId };

        _serviceMock
            .Setup(s => s.GetByCampaniaIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<PedidoListDto>>(It.IsAny<IEnumerable<PedidoCrowdfunding>>()))
            .Returns(new List<PedidoListDto>
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
    public async Task Handle_FilterByUserId_UsesGetByUserId()
    {
        // Arrange
        var userId = "user-456";
        var entity = PedidoTestData.CreateValid(userId: userId);
        var entities = new List<PedidoCrowdfunding> { entity };

        var query = new GetAllPedidosQuery { UserId = userId };

        _serviceMock
            .Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<PedidoListDto>>(It.IsAny<IEnumerable<PedidoCrowdfunding>>()))
            .Returns(new List<PedidoListDto>
            {
                new() { Id = entity.Id.Value, UserId = userId }
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        _serviceMock.Verify(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _serviceMock.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_FilterByEstado_ReturnsOnlyMatchingEstado()
    {
        // Arrange
        var pending = PedidoTestData.CreateValid(estadoPedidoId: 1);
        var completed = PedidoTestData.CreateValid(estadoPedidoId: 3);
        var entities = new List<PedidoCrowdfunding> { pending, completed };

        var query = new GetAllPedidosQuery { EstadoPedidoId = 3 };

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<PedidoListDto>>(It.Is<IEnumerable<PedidoCrowdfunding>>(
                list => list.All(p => p.EstadoPedidoId == 3))))
            .Returns(new List<PedidoListDto>
            {
                new() { Id = completed.Id.Value, EstadoPedidoId = 3 }
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Pagination_ReturnsPagedResults()
    {
        // Arrange
        var entities = new List<PedidoCrowdfunding>();
        for (int i = 0; i < 10; i++)
        {
            entities.Add(PedidoTestData.CreateValid());
        }

        var query = new GetAllPedidosQuery { PageNumber = 2, PageSize = 3 };

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<PedidoListDto>>(It.Is<IEnumerable<PedidoCrowdfunding>>(
                list => list.Count() == 3)))
            .Returns(Enumerable.Range(0, 3).Select(_ => new PedidoListDto
            {
                Id = Guid.NewGuid()
            }).ToList());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllPedidosQuery();

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PedidoCrowdfunding>());

        _mapperMock
            .Setup(m => m.Map<IEnumerable<PedidoListDto>>(It.IsAny<IEnumerable<PedidoCrowdfunding>>()))
            .Returns(new List<PedidoListDto>());

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
        var query = new GetAllPedidosQuery();

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
