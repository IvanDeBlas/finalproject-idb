using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Queries;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;
using PromoProgramaEntity = WePlayRises.Crowdpromotion.Domain.Model.PromoPrograma;
using Xunit;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.PromoTarea.Queries;

public class GetTareasPendientesQueryHandlerTests
{
    private readonly Mock<IPromoTareaService> _serviceMock;
    private readonly Mock<ILogger<GetTareasPendientesQueryHandler>> _loggerMock;
    private readonly GetTareasPendientesQueryHandler _sut;

    public GetTareasPendientesQueryHandlerTests()
    {
        _serviceMock = new Mock<IPromoTareaService>();
        _loggerMock = new Mock<ILogger<GetTareasPendientesQueryHandler>>();
        _sut = new GetTareasPendientesQueryHandler(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccess()
    {
        // Arrange
        var query = PromoTareaTestData.CreateValidTareasPendientesQuery();
        var programa = PromoTareaTestData.CreateValidPrograma();
        var items = new List<TareaPendienteItemDto>
        {
            new()
            {
                TareaPromotorId = Guid.NewGuid(),
                TareaId = Guid.NewGuid(),
                TareaNombre = "Tarea 1",
                PromotorId = Guid.NewGuid(),
                PromotorNombre = "Promotor 1",
                UrlPruebaCompletado = "https://example.com/proof",
                VecesCompletada = 1,
                FechaUltimaCompletada = DateTime.UtcNow
            }
        };

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PromoTareaTestData.DefaultArtistaId);
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);
        _serviceMock
            .Setup(s => s.GetTareasPendientesAsync(
                It.IsAny<PromoProgramaId>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((items as IReadOnlyList<TareaPendienteItemDto>, 1));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.TotalCount.Should().Be(1);
        result.Data.TotalPages.Should().Be(1);
        result.Data.Page.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = PromoTareaTestData.CreateValidTareasPendientesQuery();
        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArtistaId?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Artista);
    }

    [Fact]
    public async Task Handle_ProgramaNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = PromoTareaTestData.CreateValidTareasPendientesQuery();
        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PromoTareaTestData.DefaultArtistaId);
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromoProgramaEntity?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_PromoPrograma);
    }

    [Fact]
    public async Task Handle_NotProgramOwner_ReturnsForbidden()
    {
        // Arrange
        var query = PromoTareaTestData.CreateValidTareasPendientesQuery();
        var differentArtistaId = new ArtistaId(Guid.NewGuid());
        var programa = PromoTareaTestData.CreateValidPrograma();

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(differentArtistaId);
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);
    }

    [Fact]
    public async Task Handle_EmptyResults_ReturnsTotalPagesZero()
    {
        // Arrange
        var query = PromoTareaTestData.CreateValidTareasPendientesQuery();
        var programa = PromoTareaTestData.CreateValidPrograma();

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PromoTareaTestData.DefaultArtistaId);
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);
        _serviceMock
            .Setup(s => s.GetTareasPendientesAsync(
                It.IsAny<PromoProgramaId>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<TareaPendienteItemDto>() as IReadOnlyList<TareaPendienteItemDto>, 0));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
        result.Data.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = PromoTareaTestData.CreateValidTareasPendientesQuery();
        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
