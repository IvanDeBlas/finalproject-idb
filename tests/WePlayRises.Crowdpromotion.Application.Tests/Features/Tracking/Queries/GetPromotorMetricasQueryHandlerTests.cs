using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Queries;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;
using Xunit;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Tracking.Queries;

public class GetPromotorMetricasQueryHandlerTests
{
    private readonly Mock<IPromoEventoService> _eventoServiceMock;
    private readonly Mock<IValidator<GetPromotorMetricasQuery>> _validatorMock;
    private readonly Mock<ILogger<GetPromotorMetricasQueryHandler>> _loggerMock;
    private readonly GetPromotorMetricasQueryHandler _sut;

    public GetPromotorMetricasQueryHandlerTests()
    {
        _eventoServiceMock = new Mock<IPromoEventoService>();
        _validatorMock = new Mock<IValidator<GetPromotorMetricasQuery>>();
        _loggerMock = new Mock<ILogger<GetPromotorMetricasQueryHandler>>();

        _sut = new GetPromotorMetricasQueryHandler(
            _eventoServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsMetricas()
    {
        // Arrange
        var query = TrackingTestData.CreateValidPromotorMetricasQuery();
        var promotor = TrackingTestData.CreatePromotor();
        var metricasResponse = TrackingTestData.CreatePromotorMetricasResponse();

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _eventoServiceMock
            .Setup(s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _eventoServiceMock
            .Setup(s => s.GetPromotorMetricasAsync(
                promotor.Id,
                query.ProgramaId,
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(metricasResponse);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.PromotorId.Should().Be(metricasResponse.PromotorId);
        result.Data.PromotorNombre.Should().Be(metricasResponse.PromotorNombre);
        result.Data.Kpis.Should().NotBeNull();
        result.Data.Kpis.MisClicks.Should().Be(50);
        result.Data.Kpis.MisConversiones.Should().Be(3);

        _eventoServiceMock.Verify(
            s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()),
            Times.Once);
        _eventoServiceMock.Verify(
            s => s.GetPromotorMetricasAsync(
                promotor.Id,
                query.ProgramaId,
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var query = TrackingTestData.CreateValidPromotorMetricasQuery();

        var validationFailures = new List<ValidationFailure>
        {
            new("UserId", "El usuario es obligatorio")
            {
                ErrorCode = ServiceResponseMessageType.Validation_Required
            },
            new("FechaDesde", "El rango de fechas es invalido")
            {
                ErrorCode = ServiceResponseMessageType.Validation_FechaRangoInvalido
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().NotBeEmpty();
        result.Messages.Should().HaveCount(2);

        _eventoServiceMock.Verify(
            s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _eventoServiceMock.Verify(
            s => s.GetPromotorMetricasAsync(
                It.IsAny<BuildingBlocks.EntityFramework.StronglyTypedIds.PromotorId>(),
                It.IsAny<Guid?>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_PromotorNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = TrackingTestData.CreateValidPromotorMetricasQuery();

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _eventoServiceMock
            .Setup(s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Promotor?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Promotor);

        _eventoServiceMock.Verify(
            s => s.GetPromotorMetricasAsync(
                It.IsAny<BuildingBlocks.EntityFramework.StronglyTypedIds.PromotorId>(),
                It.IsAny<Guid?>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrowsException_ReturnsInternalError()
    {
        // Arrange
        var query = TrackingTestData.CreateValidPromotorMetricasQuery();
        var promotor = TrackingTestData.CreatePromotor();

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _eventoServiceMock
            .Setup(s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _eventoServiceMock
            .Setup(s => s.GetPromotorMetricasAsync(
                It.IsAny<BuildingBlocks.EntityFramework.StronglyTypedIds.PromotorId>(),
                It.IsAny<Guid?>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
