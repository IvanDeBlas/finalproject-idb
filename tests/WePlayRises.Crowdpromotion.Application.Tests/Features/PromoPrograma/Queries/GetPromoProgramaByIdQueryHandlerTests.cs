using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Queries;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Mapping;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.PromoPrograma.Queries;

public class GetPromoProgramaByIdQueryHandlerTests
{
    private readonly Mock<IPromoProgramaService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<GetPromoProgramaByIdQueryHandler>> _loggerMock;
    private readonly GetPromoProgramaByIdQueryHandler _sut;

    public GetPromoProgramaByIdQueryHandlerTests()
    {
        _serviceMock = new Mock<IPromoProgramaService>();
        _loggerMock = new Mock<ILogger<GetPromoProgramaByIdQueryHandler>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PromoProgramaProfile>();
            cfg.AddProfile<PromoTareaProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _sut = new GetPromoProgramaByIdQueryHandler(
            _serviceMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsDetailDto()
    {
        // Arrange
        var programa = PromoProgramaTestData.CreateValidProgramaWithTareas(
            id: PromoProgramaTestData.DefaultProgramaId,
            artistaId: PromoProgramaTestData.DefaultArtistaId);
        programa.Promotores = new List<PromoProgramaPromotor>
        {
            new()
            {
                Id = Guid.NewGuid(),
                EsActivo = true,
                FechaInscripcion = DateTime.UtcNow,
                Promotor = new Promotor
                {
                    Id = new PromotorId(Guid.NewGuid()),
                    NombrePublico = "Promotor Test",
                    TipoPromotorId = 1,
                    UserId = Guid.NewGuid().ToString(),
                    EsActivo = true,
                    FechaCreacion = DateTime.UtcNow
                }
            }
        };

        var resumenDto = new PromoProgramaResumenDto
        {
            TotalPromotoresAprobados = 1,
            TotalPromotoresPendientes = 0,
            TotalEventos = 5,
            TotalConversiones = 2,
            ValorTotalGenerado = 150.50m
        };

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(
                PromoProgramaTestData.DefaultUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PromoProgramaTestData.DefaultArtistaId);

        _serviceMock
            .Setup(s => s.GetByIdWithFullDetailAsync(
                It.Is<PromoProgramaId>(id => id.Value == PromoProgramaTestData.DefaultProgramaId.Value),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock
            .Setup(s => s.GetResumenAsync(
                It.Is<PromoProgramaId>(id => id.Value == PromoProgramaTestData.DefaultProgramaId.Value),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(resumenDto);

        var query = new GetPromoProgramaByIdQuery
        {
            Id = PromoProgramaTestData.DefaultProgramaId.Value,
            UserId = PromoProgramaTestData.DefaultUserId
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(PromoProgramaTestData.DefaultProgramaId.Value);
        result.Data.Titulo.Should().Be(programa.Titulo);
        result.Data.Tareas.Should().NotBeNull();
        result.Data.Tareas.Should().HaveCount(1);
        result.Data.Promotores.Should().NotBeNull();
        result.Data.Promotores.Should().HaveCount(1);
        result.Data.Promotores[0].PromotorNombre.Should().Be("Promotor Test");
        result.Data.Resumen.Should().NotBeNull();
        result.Data.Resumen.TotalPromotoresAprobados.Should().Be(1);
        result.Data.Resumen.TotalEventos.Should().Be(5);
        result.Data.Resumen.ValorTotalGenerado.Should().Be(150.50m);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArtistaId?)null);

        var query = new GetPromoProgramaByIdQuery
        {
            Id = PromoProgramaTestData.DefaultProgramaId.Value,
            UserId = "nonexistent-user-id"
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Artista);
    }

    [Fact]
    public async Task Handle_ProgramaNotFound_ReturnsNotFound()
    {
        // Arrange
        var nonExistentProgramaId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(
                PromoProgramaTestData.DefaultUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PromoProgramaTestData.DefaultArtistaId);

        _serviceMock
            .Setup(s => s.GetByIdWithFullDetailAsync(
                It.Is<PromoProgramaId>(id => id.Value == nonExistentProgramaId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Model.PromoPrograma?)null);

        var query = new GetPromoProgramaByIdQuery
        {
            Id = nonExistentProgramaId,
            UserId = PromoProgramaTestData.DefaultUserId
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_PromoPrograma);
    }

    [Fact]
    public async Task Handle_WrongArtista_ReturnsForbidden()
    {
        // Arrange
        var otherArtistaId = new ArtistaId(Guid.NewGuid());
        var programa = PromoProgramaTestData.CreateValidPrograma(
            id: PromoProgramaTestData.DefaultProgramaId,
            artistaId: otherArtistaId);

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(
                PromoProgramaTestData.DefaultUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PromoProgramaTestData.DefaultArtistaId);

        _serviceMock
            .Setup(s => s.GetByIdWithFullDetailAsync(
                It.Is<PromoProgramaId>(id => id.Value == PromoProgramaTestData.DefaultProgramaId.Value),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        var query = new GetPromoProgramaByIdQuery
        {
            Id = PromoProgramaTestData.DefaultProgramaId.Value,
            UserId = PromoProgramaTestData.DefaultUserId
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        var query = new GetPromoProgramaByIdQuery
        {
            Id = PromoProgramaTestData.DefaultProgramaId.Value,
            UserId = PromoProgramaTestData.DefaultUserId
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
