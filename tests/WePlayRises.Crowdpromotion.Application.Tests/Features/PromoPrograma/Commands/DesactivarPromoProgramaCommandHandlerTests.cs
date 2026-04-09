using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;
using Xunit;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.PromoPrograma.Commands;

public class DesactivarPromoProgramaCommandHandlerTests
{
    private readonly Mock<IPromoProgramaService> _serviceMock;
    private readonly Mock<ILogger<DesactivarPromoProgramaCommandHandler>> _loggerMock;
    private readonly DesactivarPromoProgramaCommandHandler _sut;

    public DesactivarPromoProgramaCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromoProgramaService>();
        _loggerMock = new Mock<ILogger<DesactivarPromoProgramaCommandHandler>>();

        _sut = new DesactivarPromoProgramaCommandHandler(
            _serviceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsDesactivadoResult()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidDesactivarCommand();
        var artistaId = PromoProgramaTestData.DefaultArtistaId;
        var programa = PromoProgramaTestData.CreateValidPrograma(
            id: new PromoProgramaId(command.Id),
            artistaId: artistaId,
            esActivo: true);
        var tareasDesactivadas = 3;

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock
            .Setup(s => s.DesactivarWithTareasAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tareasDesactivadas);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(command.Id);
        result.Data.EsActivo.Should().BeFalse();
        result.Data.TareasDesactivadas.Should().Be(tareasDesactivadas);
        result.Messages.Should().ContainSingle(m => m.HttpStatusCode == HttpStatusCode.OK);

        _serviceMock.Verify(
            s => s.DesactivarWithTareasAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NullUserId_ReturnsBadRequest()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidDesactivarCommand();
        command.UserId = null;

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Unauthorized);

        _serviceMock.Verify(
            s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ProgramaNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidDesactivarCommand();
        var artistaId = PromoProgramaTestData.DefaultArtistaId;

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Model.PromoPrograma?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_PromoPrograma);

        _serviceMock.Verify(
            s => s.DesactivarWithTareasAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ProgramaAlreadyInactive_ReturnsBusinessRuleError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidDesactivarCommand();
        var artistaId = PromoProgramaTestData.DefaultArtistaId;
        var programa = PromoProgramaTestData.CreateValidPrograma(
            id: new PromoProgramaId(command.Id),
            artistaId: artistaId,
            esActivo: false);

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_PromoProgramaAlreadyInactive);

        _serviceMock.Verify(
            s => s.DesactivarWithTareasAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidDesactivarCommand();
        var artistaId = PromoProgramaTestData.DefaultArtistaId;
        var programa = PromoProgramaTestData.CreateValidPrograma(
            id: new PromoProgramaId(command.Id),
            artistaId: artistaId,
            esActivo: true);

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock
            .Setup(s => s.DesactivarWithTareasAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_WrongArtista_ReturnsForbidden()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidDesactivarCommand();
        var artistaId = PromoProgramaTestData.DefaultArtistaId;
        var differentArtistaId = new ArtistaId(Guid.NewGuid());

        var programa = PromoProgramaTestData.CreateValidPrograma(
            id: new PromoProgramaId(command.Id),
            artistaId: differentArtistaId,
            esActivo: true);

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);

        _serviceMock.Verify(
            s => s.DesactivarWithTareasAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_EmptyUserId_ReturnsBadRequest()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidDesactivarCommand();
        command.UserId = "";

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Unauthorized);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidDesactivarCommand();

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArtistaId?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Artista);

        _serviceMock.Verify(
            s => s.GetByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
