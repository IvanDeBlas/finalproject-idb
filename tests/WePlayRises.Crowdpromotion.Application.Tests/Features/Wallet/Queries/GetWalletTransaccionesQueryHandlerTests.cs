using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Mapping;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Wallet.Queries;

public class GetWalletTransaccionesQueryHandlerTests
{
    private readonly Mock<IPromotorWalletService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<GetWalletTransaccionesQuery>> _validatorMock;
    private readonly Mock<ILogger<GetWalletTransaccionesQueryHandler>> _loggerMock;
    private readonly GetWalletTransaccionesQueryHandler _sut;

    public GetWalletTransaccionesQueryHandlerTests()
    {
        _serviceMock = new Mock<IPromotorWalletService>();
        _validatorMock = new Mock<IValidator<GetWalletTransaccionesQuery>>();
        _loggerMock = new Mock<ILogger<GetWalletTransaccionesQueryHandler>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PromotorWalletProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _sut = new GetWalletTransaccionesQueryHandler(
            _serviceMock.Object,
            _mapper,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsPagedTransacciones()
    {
        // Arrange
        var query = WalletTestData.CreateGetTransaccionesQuery();
        var promotor = WalletTestData.CreatePromotor();
        var wallet = WalletTestData.CreateWallet();
        var transacciones = WalletTestData.CreateTransacciones(3, wallet.Id);

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock
            .Setup(s => s.GetWalletConMonedaAsync(promotor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((wallet, "EUR"));

        _serviceMock
            .Setup(s => s.GetTransaccionesPagedAsync(
                wallet.Id, null, null, null, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((transacciones.AsReadOnly(), 3));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(3);
        result.Data.TotalCount.Should().Be(3);
        result.Data.Page.Should().Be(1);
        result.Data.PageSize.Should().Be(10);
        result.Data.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPage()
    {
        // Arrange
        var query = WalletTestData.CreateGetTransaccionesQuery();
        var promotor = WalletTestData.CreatePromotor();
        var wallet = WalletTestData.CreateWallet();

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock
            .Setup(s => s.GetWalletConMonedaAsync(promotor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((wallet, "EUR"));

        _serviceMock
            .Setup(s => s.GetTransaccionesPagedAsync(
                wallet.Id, null, null, null, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<PromotorWalletTransaccion>().AsReadOnly(), 0));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
        result.Data.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var query = new GetWalletTransaccionesQuery { UserId = string.Empty, Page = 0 };
        var failures = new List<ValidationFailure>
        {
            new("UserId", "El identificador de usuario es obligatorio")
            {
                ErrorCode = ServiceResponseMessageType.Validation_Required
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Handle_PromotorNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = WalletTestData.CreateGetTransaccionesQuery();

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Promotor?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Promotor);
    }

    [Fact]
    public async Task Handle_WalletNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = WalletTestData.CreateGetTransaccionesQuery();
        var promotor = WalletTestData.CreatePromotor();

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock
            .Setup(s => s.GetWalletConMonedaAsync(promotor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(((PromotorWallet?)null, (string?)null));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Wallet);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var query = WalletTestData.CreateGetTransaccionesQuery();

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
