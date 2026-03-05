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

public class GetMisTareasQueryHandlerTests
{
    private readonly Mock<IPromoTareaService> _serviceMock;
    private readonly Mock<ILogger<GetMisTareasQueryHandler>> _loggerMock;
    private readonly GetMisTareasQueryHandler _sut;

    public GetMisTareasQueryHandlerTests()
    {
        _serviceMock = new Mock<IPromoTareaService>();
        _loggerMock = new Mock<ILogger<GetMisTareasQueryHandler>>();
        _sut = new GetMisTareasQueryHandler(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccess()
    {
        // Arrange
        var query = PromoTareaTestData.CreateValidMisTareasQuery();
        var promotor = PromoTareaTestData.CreateValidPromotor();
        var programa = PromoTareaTestData.CreateValidPrograma();
        var inscripcion = PromoTareaTestData.CreateValidInscripcion();
        var responseDto = new MisTareasResponseDto
        {
            ProgramaId = programa.Id.Value,
            ProgramaTitulo = programa.Titulo,
            Items = new List<MisTareasItemDto>()
        };

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);
        _serviceMock
            .Setup(s => s.GetInscripcionAprobadaAsync(
                It.IsAny<PromotorId>(), It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(inscripcion);
        _serviceMock
            .Setup(s => s.GetTareasActivasConEstadoAsync(
                It.IsAny<PromoProgramaId>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ProgramaTitulo.Should().Be(programa.Titulo);
    }

    [Fact]
    public async Task Handle_PromotorNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = PromoTareaTestData.CreateValidMisTareasQuery();
        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Promotor?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Promotor);
    }

    [Fact]
    public async Task Handle_ProgramaNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = PromoTareaTestData.CreateValidMisTareasQuery();
        var promotor = PromoTareaTestData.CreateValidPromotor();

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);
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
    public async Task Handle_NoInscripcionAprobada_ReturnsForbidden()
    {
        // Arrange
        var query = PromoTareaTestData.CreateValidMisTareasQuery();
        var promotor = PromoTareaTestData.CreateValidPromotor();
        var programa = PromoTareaTestData.CreateValidPrograma();

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);
        _serviceMock
            .Setup(s => s.GetInscripcionAprobadaAsync(
                It.IsAny<PromotorId>(), It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromoProgramaPromotor?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_InscripcionBloqueada_ReturnsForbidden()
    {
        // Arrange
        var query = PromoTareaTestData.CreateValidMisTareasQuery();
        var promotor = PromoTareaTestData.CreateValidPromotor();
        var programa = PromoTareaTestData.CreateValidPrograma();
        var inscripcion = PromoTareaTestData.CreateValidInscripcion(esBloqueado: true);

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);
        _serviceMock
            .Setup(s => s.GetInscripcionAprobadaAsync(
                It.IsAny<PromotorId>(), It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(inscripcion);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = PromoTareaTestData.CreateValidMisTareasQuery();
        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
