using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
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

public class GetPromotorWalletQueryHandlerTests
{
    private readonly Mock<IPromotorWalletService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<GetPromotorWalletQuery>> _validatorMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<GetPromotorWalletQueryHandler>> _loggerMock;
    private readonly GetPromotorWalletQueryHandler _sut;

    public GetPromotorWalletQueryHandlerTests()
    {
        _serviceMock = new Mock<IPromotorWalletService>();
        _validatorMock = new Mock<IValidator<GetPromotorWalletQuery>>();
        _configMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<GetPromotorWalletQueryHandler>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PromotorWalletProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _configMock
            .Setup(c => c.GetSection("Crowdpromotion:MinimoRetiro").Value)
            .Returns("10.00");

        _sut = new GetPromotorWalletQueryHandler(
            _serviceMock.Object,
            _mapper,
            _validatorMock.Object,
            _configMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsWalletDto()
    {
        // Arrange
        var query = WalletTestData.CreateGetWalletQuery();
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

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.WalletId.Should().Be(wallet.Id);
        result.Data.SaldoDisponible.Should().Be(wallet.SaldoDisponible);
        result.Data.MonedaNombre.Should().Be("EUR");
        result.Data.MinimoRetiro.Should().Be(10.00m);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var query = new GetPromotorWalletQuery { UserId = string.Empty };
        var validationFailures = new List<ValidationFailure>
        {
            new("UserId", "El identificador de usuario es obligatorio")
            {
                ErrorCode = ServiceResponseMessageType.Validation_Required
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Validation_Required);
        _serviceMock.Verify(
            s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_PromotorNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = WalletTestData.CreateGetWalletQuery();

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
        var query = WalletTestData.CreateGetWalletQuery();
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
        var query = WalletTestData.CreateGetWalletQuery();

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB connection failed"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
