using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Commands;

public class AceptarPropuestaCommandHandlerTests
{
    private readonly Mock<IAcuerdoCrowdsourcingService> _acuerdoServiceMock;
    private readonly Mock<IPropuestaCrowdsourcingService> _propuestaServiceMock;
    private readonly Mock<INecesidadCrowdsourcingService> _necesidadServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IValidator<AceptarPropuestaCommand>> _validatorMock;
    private readonly Mock<ILogger<AceptarPropuestaCommandHandler>> _loggerMock;
    private readonly AceptarPropuestaCommandHandler _sut;

    public AceptarPropuestaCommandHandlerTests()
    {
        _acuerdoServiceMock = new Mock<IAcuerdoCrowdsourcingService>();
        _propuestaServiceMock = new Mock<IPropuestaCrowdsourcingService>();
        _necesidadServiceMock = new Mock<INecesidadCrowdsourcingService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _validatorMock = new Mock<IValidator<AceptarPropuestaCommand>>();
        _loggerMock = new Mock<ILogger<AceptarPropuestaCommandHandler>>();
        _sut = new AceptarPropuestaCommandHandler(
            _acuerdoServiceMock.Object,
            _propuestaServiceMock.Object,
            _necesidadServiceMock.Object,
            _artistaServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<AceptarPropuestaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAcuerdoSuccessfully()
    {
        // Arrange
        // NecesidadCrowdsourcingController stores UserId (not Artista entity PK) as ArtistaId
        var userId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(userId: userId.ToString());
        var necesidad = CrowdsourcingTestData.CreateNecesidadAbierta(userId);
        var propuesta = CrowdsourcingTestData.CreatePropuestaPendiente(necesidad.Id);
        var acuerdoId = AcuerdoCrowdsourcingId.CreateNew();
        var conversacionId = Guid.NewGuid();

        var command = new AceptarPropuestaCommand
        {
            PropuestaId = propuesta.Id.Value,
            UserId = userId.ToString(),
            TituloInterno = "Mi acuerdo",
            FechaInicio = DateTime.UtcNow
        };

        SetupValidValidation();
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);
        _propuestaServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propuesta);
        _necesidadServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);
        _acuerdoServiceMock.Setup(s => s.AceptarPropuestaAsync(
                It.IsAny<AcuerdoCrowdsourcing>(), It.IsAny<PropuestaCrowdsourcing>(),
                It.IsAny<NecesidadCrowdsourcing>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((acuerdoId, 2, conversacionId));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.AcuerdoId.Should().Be(acuerdoId.Value);
        result.Data.EstadoAcuerdoNombre.Should().Be("Activo");
        result.Data.ConversacionId.Should().Be(conversacionId);
        result.Data.PropuestasRechazadas.Should().Be(2);
        result.Messages.Should().ContainSingle(m => m.HttpStatusCode == System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsErrors()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("TituloInterno", "El titulo interno es obligatorio")
            { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<AceptarPropuestaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _sut.Handle(new AceptarPropuestaCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _acuerdoServiceMock.Verify(s => s.AceptarPropuestaAsync(
            It.IsAny<AcuerdoCrowdsourcing>(), It.IsAny<PropuestaCrowdsourcing>(),
            It.IsAny<NecesidadCrowdsourcing>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsForbidden()
    {
        // Arrange
        SetupValidValidation();
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAccess.Domain.Model.Artista?)null);

        // Act
        var result = await _sut.Handle(new AceptarPropuestaCommand { UserId = "unknown" }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var artista = CrowdsourcingTestData.CreateArtista();
        var userIdGuid = Guid.Parse(artista.UserIdPropietario);
        var necesidad = CrowdsourcingTestData.CreateNecesidadAbierta(userIdGuid);
        var propuesta = CrowdsourcingTestData.CreatePropuestaPendiente(necesidad.Id);

        var command = new AceptarPropuestaCommand
        {
            PropuestaId = propuesta.Id.Value,
            UserId = artista.UserIdPropietario,
            TituloInterno = "Test",
            FechaInicio = DateTime.UtcNow
        };

        SetupValidValidation();
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);
        _propuestaServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propuesta);
        _necesidadServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);
        _acuerdoServiceMock.Setup(s => s.AceptarPropuestaAsync(
                It.IsAny<AcuerdoCrowdsourcing>(), It.IsAny<PropuestaCrowdsourcing>(),
                It.IsAny<NecesidadCrowdsourcing>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
