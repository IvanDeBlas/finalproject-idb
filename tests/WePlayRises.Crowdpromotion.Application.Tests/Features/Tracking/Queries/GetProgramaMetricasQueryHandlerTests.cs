using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Queries;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;
using PromoProgramaEntity = WePlayRises.Crowdpromotion.Domain.Model.PromoPrograma;
using Xunit;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Tracking.Queries;

public class GetProgramaMetricasQueryHandlerTests
{
    private readonly Mock<IPromoEventoService> _eventoServiceMock;
    private readonly Mock<IPromoProgramaService> _programaServiceMock;
    private readonly Mock<IValidator<GetProgramaMetricasQuery>> _validatorMock;
    private readonly Mock<ILogger<GetProgramaMetricasQueryHandler>> _loggerMock;
    private readonly GetProgramaMetricasQueryHandler _sut;

    public GetProgramaMetricasQueryHandlerTests()
    {
        _eventoServiceMock = new Mock<IPromoEventoService>();
        _programaServiceMock = new Mock<IPromoProgramaService>();
        _validatorMock = new Mock<IValidator<GetProgramaMetricasQuery>>();
        _loggerMock = new Mock<ILogger<GetProgramaMetricasQueryHandler>>();

        _sut = new GetProgramaMetricasQueryHandler(
            _eventoServiceMock.Object,
            _programaServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsMetricas()
    {
        // Arrange
        var query = TrackingTestData.CreateValidProgramaMetricasQuery();
        var artistaId = TrackingTestData.DefaultArtistaId;
        var programa = TrackingTestData.CreatePromoPrograma(artistaId);
        var metricasResponse = TrackingTestData.CreateProgramaMetricasResponse();

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _programaServiceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _eventoServiceMock
            .Setup(s => s.GetProgramaByIdAsync(query.ProgramaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _eventoServiceMock
            .Setup(s => s.GetProgramaMetricasAsync(
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
        result.Data!.ProgramaId.Should().Be(metricasResponse.ProgramaId);
        result.Data.ProgramaTitulo.Should().Be(metricasResponse.ProgramaTitulo);
        result.Data.Kpis.Should().NotBeNull();
        result.Data.Kpis.TotalClicks.Should().Be(100);
        result.Data.Kpis.TotalConversiones.Should().Be(5);

        _programaServiceMock.Verify(
            s => s.GetArtistaIdByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()),
            Times.Once);
        _eventoServiceMock.Verify(
            s => s.GetProgramaByIdAsync(query.ProgramaId, It.IsAny<CancellationToken>()),
            Times.Once);
        _eventoServiceMock.Verify(
            s => s.GetProgramaMetricasAsync(
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
        var query = TrackingTestData.CreateValidProgramaMetricasQuery();

        var validationFailures = new List<ValidationFailure>
        {
            new("ProgramaId", "El programa es obligatorio")
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

        _programaServiceMock.Verify(
            s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _eventoServiceMock.Verify(
            s => s.GetProgramaByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _eventoServiceMock.Verify(
            s => s.GetProgramaMetricasAsync(
                It.IsAny<Guid>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = TrackingTestData.CreateValidProgramaMetricasQuery();

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _programaServiceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArtistaId?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Artista);

        _eventoServiceMock.Verify(
            s => s.GetProgramaByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _eventoServiceMock.Verify(
            s => s.GetProgramaMetricasAsync(
                It.IsAny<Guid>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ProgramaNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = TrackingTestData.CreateValidProgramaMetricasQuery();
        var artistaId = TrackingTestData.DefaultArtistaId;

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _programaServiceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _eventoServiceMock
            .Setup(s => s.GetProgramaByIdAsync(query.ProgramaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromoProgramaEntity?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_PromoPrograma);

        _eventoServiceMock.Verify(
            s => s.GetProgramaMetricasAsync(
                It.IsAny<Guid>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_NotOwner_ReturnsForbidden()
    {
        // Arrange
        var query = TrackingTestData.CreateValidProgramaMetricasQuery();
        var artistaId = TrackingTestData.DefaultArtistaId;
        var differentArtistaId = new ArtistaId(Guid.NewGuid());
        var programa = TrackingTestData.CreatePromoPrograma(differentArtistaId);

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _programaServiceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _eventoServiceMock
            .Setup(s => s.GetProgramaByIdAsync(query.ProgramaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);

        _eventoServiceMock.Verify(
            s => s.GetProgramaMetricasAsync(
                It.IsAny<Guid>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrowsException_ReturnsInternalError()
    {
        // Arrange
        var query = TrackingTestData.CreateValidProgramaMetricasQuery();
        var artistaId = TrackingTestData.DefaultArtistaId;
        var programa = TrackingTestData.CreatePromoPrograma(artistaId);

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _programaServiceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _eventoServiceMock
            .Setup(s => s.GetProgramaByIdAsync(query.ProgramaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _eventoServiceMock
            .Setup(s => s.GetProgramaMetricasAsync(
                It.IsAny<Guid>(),
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
