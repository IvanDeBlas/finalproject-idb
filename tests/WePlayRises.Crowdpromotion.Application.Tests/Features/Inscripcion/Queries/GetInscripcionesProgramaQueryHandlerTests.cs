using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Queries;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Mapping;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;
using PromoProgramaEntity = WePlayRises.Crowdpromotion.Domain.Model.PromoPrograma;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Inscripcion.Queries;

public class GetInscripcionesProgramaQueryHandlerTests
{
    private readonly Mock<IInscripcionService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<GetInscripcionesProgramaQueryHandler>> _loggerMock;
    private readonly GetInscripcionesProgramaQueryHandler _sut;

    public GetInscripcionesProgramaQueryHandlerTests()
    {
        _serviceMock = new Mock<IInscripcionService>();
        _loggerMock = new Mock<ILogger<GetInscripcionesProgramaQueryHandler>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InscripcionProfile>());
        _mapper = config.CreateMapper();

        _sut = new GetInscripcionesProgramaQueryHandler(
            _serviceMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccess()
    {
        // Arrange
        var query = new GetInscripcionesProgramaQuery
        {
            ProgramaId = InscripcionTestData.DefaultProgramaId.Value,
            UserId = InscripcionTestData.DefaultUserId,
            Page = 1,
            PageSize = 10
        };
        var programa = InscripcionTestData.CreateValidPrograma(artistaId: InscripcionTestData.DefaultArtistaId);
        var inscripciones = new List<PromoProgramaPromotor>
        {
            InscripcionTestData.CreatePendienteInscripcion()
        };

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(InscripcionTestData.DefaultArtistaId);

        _serviceMock.Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock.Setup(s => s.GetInscripcionesPorProgramaAsync(
                It.IsAny<PromoProgramaId>(), It.IsAny<string?>(), query.Page, query.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync((inscripciones as IReadOnlyList<PromoProgramaPromotor>, 1));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalCount.Should().Be(1);
        result.Data.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = new GetInscripcionesProgramaQuery
        {
            ProgramaId = InscripcionTestData.DefaultProgramaId.Value,
            UserId = InscripcionTestData.DefaultUserId,
            Page = 1,
            PageSize = 10
        };

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArtistaId?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Artista);
    }

    [Fact]
    public async Task Handle_ProgramaNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = new GetInscripcionesProgramaQuery
        {
            ProgramaId = InscripcionTestData.DefaultProgramaId.Value,
            UserId = InscripcionTestData.DefaultUserId,
            Page = 1,
            PageSize = 10
        };

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(InscripcionTestData.DefaultArtistaId);

        _serviceMock.Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromoProgramaEntity?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.NotFound_PromoPrograma);
    }

    [Fact]
    public async Task Handle_NotOwner_ReturnsForbidden()
    {
        // Arrange
        var query = new GetInscripcionesProgramaQuery
        {
            ProgramaId = InscripcionTestData.DefaultProgramaId.Value,
            UserId = InscripcionTestData.DefaultUserId,
            Page = 1,
            PageSize = 10
        };
        var differentArtistaId = new ArtistaId(Guid.NewGuid());
        var programa = InscripcionTestData.CreateValidPrograma(artistaId: differentArtistaId);

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(InscripcionTestData.DefaultArtistaId);

        _serviceMock.Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalServerError()
    {
        // Arrange
        var query = new GetInscripcionesProgramaQuery
        {
            ProgramaId = InscripcionTestData.DefaultProgramaId.Value,
            UserId = InscripcionTestData.DefaultUserId,
            Page = 1,
            PageSize = 10
        };

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
