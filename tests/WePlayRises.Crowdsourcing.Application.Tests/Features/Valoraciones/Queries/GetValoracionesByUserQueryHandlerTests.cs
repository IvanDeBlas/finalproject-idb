using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Queries;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Valoraciones.Queries;

public class GetValoracionesByUserQueryHandlerTests
{
    private readonly Mock<IValoracionCrowdsourcingService> _valoracionServiceMock;
    private readonly Mock<IValidator<GetValoracionesByUserQuery>> _validatorMock;
    private readonly Mock<ILogger<GetValoracionesByUserQueryHandler>> _loggerMock;
    private readonly GetValoracionesByUserQueryHandler _sut;

    private readonly string _userId = Guid.NewGuid().ToString();

    public GetValoracionesByUserQueryHandlerTests()
    {
        _valoracionServiceMock = new Mock<IValoracionCrowdsourcingService>();
        _validatorMock = new Mock<IValidator<GetValoracionesByUserQuery>>();
        _loggerMock = new Mock<ILogger<GetValoracionesByUserQueryHandler>>();

        _sut = new GetValoracionesByUserQueryHandler(
            _valoracionServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetValoracionesByUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private GetValoracionesByUserQuery CreateValidQuery()
    {
        return new GetValoracionesByUserQuery
        {
            UserId = _userId,
            Page = 1,
            PageSize = 10
        };
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsValoracionesUsuario()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation();

        _valoracionServiceMock
            .Setup(s => s.UsuarioExisteAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var resumen = new ValoracionResumenDto
        {
            PuntuacionMedia = 4.5m,
            TotalValoraciones = 3,
            Distribucion = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 1 }, { 5, 2 } }
        };
        _valoracionServiceMock
            .Setup(s => s.GetResumenByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resumen);

        var valoraciones = new PaginatedResponse<ValoracionListItemDto>
        {
            Items = new List<ValoracionListItemDto>
            {
                new() { Id = Guid.NewGuid(), Puntuacion = 5, AutorNombre = "Test", AcuerdoTituloInterno = "Acuerdo 1", FechaCreacion = DateTime.UtcNow }
            },
            TotalCount = 3,
            Page = 1,
            PageSize = 10
        };
        _valoracionServiceMock
            .Setup(s => s.GetByUserIdPagedAsync(_userId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(valoraciones);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.HasErrors.Should().BeFalse();
        result.Data.Should().NotBeNull();
        result.Data!.Resumen.PuntuacionMedia.Should().Be(4.5m);
        result.Data.Resumen.TotalValoraciones.Should().Be(3);
        result.Data.Valoraciones.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsErrors()
    {
        // Arrange
        var query = CreateValidQuery();
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetValoracionesByUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("UserId", "El identificador de usuario es obligatorio")
                {
                    ErrorCode = ServiceResponseMessageType.Validation_Required
                }
            }));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.HasErrors.Should().BeTrue();
        _valoracionServiceMock.Verify(
            s => s.GetResumenByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation();

        _valoracionServiceMock
            .Setup(s => s.UsuarioExisteAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Entity);
    }

    [Fact]
    public async Task Handle_EmptyResults_ReturnsSuccessWithEmptyData()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation();

        _valoracionServiceMock
            .Setup(s => s.UsuarioExisteAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _valoracionServiceMock
            .Setup(s => s.GetResumenByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValoracionResumenDto
            {
                PuntuacionMedia = null,
                TotalValoraciones = 0,
                Distribucion = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } }
            });

        _valoracionServiceMock
            .Setup(s => s.GetByUserIdPagedAsync(_userId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedResponse<ValoracionListItemDto>
            {
                Items = new List<ValoracionListItemDto>(),
                TotalCount = 0,
                Page = 1,
                PageSize = 10
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.HasErrors.Should().BeFalse();
        result.Data!.Resumen.PuntuacionMedia.Should().BeNull();
        result.Data.Resumen.TotalValoraciones.Should().Be(0);
        result.Data.Valoraciones.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_UnexpectedException_ReturnsInternalError()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation();

        _valoracionServiceMock
            .Setup(s => s.UsuarioExisteAsync(_userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_PageSizeExceeds50_CapsAt50()
    {
        // Arrange
        var query = CreateValidQuery();
        query.PageSize = 100;
        SetupValidValidation();

        _valoracionServiceMock
            .Setup(s => s.UsuarioExisteAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _valoracionServiceMock
            .Setup(s => s.GetResumenByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValoracionResumenDto { Distribucion = new() });

        _valoracionServiceMock
            .Setup(s => s.GetByUserIdPagedAsync(_userId, 1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedResponse<ValoracionListItemDto>
            {
                Items = new List<ValoracionListItemDto>(),
                TotalCount = 0,
                Page = 1,
                PageSize = 50
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        _valoracionServiceMock.Verify(
            s => s.GetByUserIdPagedAsync(_userId, 1, 50, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
