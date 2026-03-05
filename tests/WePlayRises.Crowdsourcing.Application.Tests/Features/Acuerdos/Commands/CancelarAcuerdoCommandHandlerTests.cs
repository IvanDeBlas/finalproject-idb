using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Commands;

public class CancelarAcuerdoCommandHandlerTests
{
    private readonly Mock<IAcuerdoCrowdsourcingService> _acuerdoServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IValidator<CancelarAcuerdoCommand>> _validatorMock;
    private readonly Mock<ILogger<CancelarAcuerdoCommandHandler>> _loggerMock;
    private readonly CancelarAcuerdoCommandHandler _sut;

    public CancelarAcuerdoCommandHandlerTests()
    {
        _acuerdoServiceMock = new Mock<IAcuerdoCrowdsourcingService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _validatorMock = new Mock<IValidator<CancelarAcuerdoCommand>>();
        _loggerMock = new Mock<ILogger<CancelarAcuerdoCommandHandler>>();
        _sut = new CancelarAcuerdoCommandHandler(
            _acuerdoServiceMock.Object,
            _artistaServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CancelarAcuerdoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ArtistaCancels_Success()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(artistaId, "user-artista");
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(artistaId, "user-proveedor");

        var command = new CancelarAcuerdoCommand
        {
            AcuerdoId = acuerdo.Id.Value,
            UserId = "user-artista",
            Motivo = "No puedo continuar por motivos personales, lo lamento mucho."
        };

        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-artista", It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.EstadoAcuerdoNombre.Should().Be("Cancelado");
        result.Data.NecesidadEstadoNombre.Should().Be("Abierta");
        _acuerdoServiceMock.Verify(s => s.CancelarAsync(
            It.IsAny<AcuerdoCrowdsourcingId>(), command.Motivo, "user-artista", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProveedorCancels_Success()
    {
        // Arrange
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(userIdProveedor: "user-proveedor");

        var command = new CancelarAcuerdoCommand
        {
            AcuerdoId = acuerdo.Id.Value,
            UserId = "user-proveedor",
            Motivo = "No puedo continuar por motivos personales, lo lamento mucho."
        };

        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_NonParticipant_ReturnsForbidden()
    {
        // Arrange
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo();
        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAccess.Domain.Model.Artista?)null);

        // Act
        var result = await _sut.Handle(new CancelarAcuerdoCommand
        {
            AcuerdoId = acuerdo.Id.Value, UserId = "stranger", Motivo = "x".PadRight(20, 'x')
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(userIdProveedor: "user-proveedor");
        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _acuerdoServiceMock.Setup(s => s.CancelarAsync(
                It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(new CancelarAcuerdoCommand
        {
            AcuerdoId = acuerdo.Id.Value, UserId = "user-proveedor", Motivo = "x".PadRight(20, 'x')
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
