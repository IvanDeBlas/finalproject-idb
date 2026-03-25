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

public class GetPedidoByIdQueryHandlerTests
{
    private readonly Mock<IPedidoService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetPedidoByIdQueryHandler>> _loggerMock;
    private readonly GetPedidoByIdQueryHandler _sut;

    public GetPedidoByIdQueryHandlerTests()
    {
        _serviceMock = new Mock<IPedidoService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetPedidoByIdQueryHandler>>();
        _sut = new GetPedidoByIdQueryHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingPedido_ReturnsPedidoDto()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var query = new GetPedidoByIdQuery { Id = pedidoId };
        var entity = PedidoTestData.CreateValid(pedidoId: pedidoId);
        var expectedDto = new PedidoDto
        {
            Id = pedidoId,
            CampaniaId = entity.CampaniaId.Value,
            ImporteTotal = entity.ImporteTotal
        };

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PedidoCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(m => m.Map<PedidoDto>(entity))
            .Returns(expectedDto);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(pedidoId);
        result.Data.ImporteTotal.Should().Be(entity.ImporteTotal);
    }

    [Fact]
    public async Task Handle_NonExistentPedido_ReturnsNotFound()
    {
        // Arrange
        var query = new GetPedidoByIdQuery { Id = Guid.NewGuid() };

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PedidoCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PedidoCrowdfunding?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Backing);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetPedidoByIdQuery { Id = Guid.NewGuid() };

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PedidoCrowdfundingId>(), It.IsAny<CancellationToken>()))
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
