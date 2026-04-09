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
using PromoTareaEntity = WePlayRises.Crowdpromotion.Domain.Model.PromoTarea;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.PromoPrograma.Queries;

public class GetMisProgramasQueryHandlerTests
{
    private readonly Mock<IPromoProgramaService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<GetMisProgramasQueryHandler>> _loggerMock;
    private readonly GetMisProgramasQueryHandler _sut;

    public GetMisProgramasQueryHandlerTests()
    {
        _serviceMock = new Mock<IPromoProgramaService>();
        _loggerMock = new Mock<ILogger<GetMisProgramasQueryHandler>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PromoProgramaProfile>();
            cfg.AddProfile<PromoTareaProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _sut = new GetMisProgramasQueryHandler(
            _serviceMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsListResult()
    {
        // Arrange
        var programa1 = PromoProgramaTestData.CreateValidPrograma(
            id: PromoProgramaId.CreateNew(),
            artistaId: PromoProgramaTestData.DefaultArtistaId);
        programa1.Promotores = new List<PromoProgramaPromotor>
        {
            new() { Id = Guid.NewGuid(), EsActivo = true, FechaInscripcion = DateTime.UtcNow }
        };
        programa1.Tareas = new List<PromoTareaEntity>
        {
            new() { Id = Guid.NewGuid(), Titulo = "Tarea 1", TipoEventoPromoId = 1, EsActivo = true, Orden = 1, FechaCreacion = DateTime.UtcNow }
        };

        var programa2 = PromoProgramaTestData.CreateValidPrograma(
            id: PromoProgramaId.CreateNew(),
            artistaId: PromoProgramaTestData.DefaultArtistaId);

        var items = new List<Domain.Model.PromoPrograma> { programa1, programa2 };

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(
                PromoProgramaTestData.DefaultUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PromoProgramaTestData.DefaultArtistaId);

        _serviceMock
            .Setup(s => s.GetMisProgramasAsync(
                PromoProgramaTestData.DefaultArtistaId,
                null, 1, 10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Domain.Model.PromoPrograma>)items, 2));

        var query = new GetMisProgramasQuery
        {
            UserId = PromoProgramaTestData.DefaultUserId,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
        result.Data.Page.Should().Be(1);
        result.Data.PageSize.Should().Be(10);
        result.Data.Items[0].Id.Should().Be(programa1.Id.Value);
        result.Data.Items[0].NumeroPromotores.Should().Be(1);
        result.Data.Items[0].NumeroTareas.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArtistaId?)null);

        var query = new GetMisProgramasQuery
        {
            UserId = "nonexistent-user-id",
            Page = 1,
            PageSize = 10
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
    public async Task Handle_EmptyResults_ReturnsEmptyList()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(
                PromoProgramaTestData.DefaultUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PromoProgramaTestData.DefaultArtistaId);

        _serviceMock
            .Setup(s => s.GetMisProgramasAsync(
                PromoProgramaTestData.DefaultArtistaId,
                null, 1, 10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Domain.Model.PromoPrograma>)new List<Domain.Model.PromoPrograma>(), 0));

        var query = new GetMisProgramasQuery
        {
            UserId = PromoProgramaTestData.DefaultUserId,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        var query = new GetMisProgramasQuery
        {
            UserId = PromoProgramaTestData.DefaultUserId,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
