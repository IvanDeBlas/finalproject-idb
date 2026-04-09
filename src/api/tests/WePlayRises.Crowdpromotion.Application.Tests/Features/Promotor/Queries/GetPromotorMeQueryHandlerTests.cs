using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Queries;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Mapping;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Promotor.Queries;

public class GetPromotorMeQueryHandlerTests
{
    private readonly Mock<IPromotorService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<GetPromotorMeQueryHandler>> _loggerMock;
    private readonly GetPromotorMeQueryHandler _sut;

    public GetPromotorMeQueryHandlerTests()
    {
        _serviceMock = new Mock<IPromotorService>();
        _loggerMock = new Mock<ILogger<GetPromotorMeQueryHandler>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<PromotorProfile>());
        _mapper = config.CreateMapper();

        _sut = new GetPromotorMeQueryHandler(
            _serviceMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_PromotorExists_ReturnsFullProfile()
    {
        // Arrange
        var userId = PromotorTestData.DefaultUserId;
        var query = new GetPromotorMeQuery { UserId = userId };
        var promotor = PromotorTestData.CreateValidPromotor(userId: userId);
        var wallet = PromotorTestData.CreateWalletEur(promotor.Id);

        _serviceMock.Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock.Setup(s => s.GetProgramasActivosCountAsync(promotor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        _serviceMock.Setup(s => s.GetWalletEurAsync(promotor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.NombrePublico.Should().Be("Test Promotor");
        result.Data.TotalProgramasActivos.Should().Be(5);
        result.Data.TotalComisionesGanadas.Should().Be(500.00m);
        result.Data.MonedaComisiones.Should().Be("EUR");
    }

    [Fact]
    public async Task Handle_PromotorNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = new GetPromotorMeQuery { UserId = "unknown-user" };

        _serviceMock.Setup(s => s.GetByUserIdAsync("unknown-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Model.Promotor?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Promotor);
    }

    [Fact]
    public async Task Handle_NoWallet_ReturnsZeroComisiones()
    {
        // Arrange
        var userId = PromotorTestData.DefaultUserId;
        var query = new GetPromotorMeQuery { UserId = userId };
        var promotor = PromotorTestData.CreateValidPromotor(userId: userId);

        _serviceMock.Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock.Setup(s => s.GetProgramasActivosCountAsync(promotor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _serviceMock.Setup(s => s.GetWalletEurAsync(promotor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Model.PromotorWallet?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.TotalComisionesGanadas.Should().Be(0m);
        result.Data.TotalProgramasActivos.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalServerError()
    {
        // Arrange
        var query = new GetPromotorMeQuery { UserId = "user1" };

        _serviceMock.Setup(s => s.GetByUserIdAsync("user1", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
