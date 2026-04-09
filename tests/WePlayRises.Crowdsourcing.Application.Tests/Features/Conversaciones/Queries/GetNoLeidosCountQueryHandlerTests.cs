using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Queries;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Conversaciones.Queries;

public class GetNoLeidosCountQueryHandlerTests
{
    private readonly Mock<IMensajeCrowdsourcingService> _mensajeServiceMock;
    private readonly Mock<IValidator<GetNoLeidosCountQuery>> _validatorMock;
    private readonly Mock<ILogger<GetNoLeidosCountQueryHandler>> _loggerMock;
    private readonly GetNoLeidosCountQueryHandler _sut;

    public GetNoLeidosCountQueryHandlerTests()
    {
        _mensajeServiceMock = new Mock<IMensajeCrowdsourcingService>();
        _validatorMock = new Mock<IValidator<GetNoLeidosCountQuery>>();
        _loggerMock = new Mock<ILogger<GetNoLeidosCountQueryHandler>>();
        _sut = new GetNoLeidosCountQueryHandler(
            _mensajeServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetNoLeidosCountQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsTotalNoLeidos()
    {
        // Arrange
        var query = new GetNoLeidosCountQuery { UserId = Guid.NewGuid().ToString() };
        SetupValidValidation();

        _mensajeServiceMock
            .Setup(s => s.GetTotalNoLeidosAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.TotalNoLeidos.Should().Be(5);
    }

    [Fact]
    public async Task Handle_ZeroUnread_ReturnsZero()
    {
        // Arrange
        var query = new GetNoLeidosCountQuery { UserId = Guid.NewGuid().ToString() };
        SetupValidValidation();

        _mensajeServiceMock
            .Setup(s => s.GetTotalNoLeidosAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.TotalNoLeidos.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetNoLeidosCountQuery { UserId = Guid.NewGuid().ToString() };
        SetupValidValidation();

        _mensajeServiceMock
            .Setup(s => s.GetTotalNoLeidosAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
