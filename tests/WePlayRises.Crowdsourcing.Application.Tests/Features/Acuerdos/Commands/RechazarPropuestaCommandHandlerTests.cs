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
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Commands;

public class RechazarPropuestaCommandHandlerTests
{
    private readonly Mock<IPropuestaCrowdsourcingService> _propuestaServiceMock;
    private readonly Mock<INecesidadCrowdsourcingService> _necesidadServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IValidator<RechazarPropuestaCommand>> _validatorMock;
    private readonly Mock<ILogger<RechazarPropuestaCommandHandler>> _loggerMock;
    private readonly RechazarPropuestaCommandHandler _sut;

    public RechazarPropuestaCommandHandlerTests()
    {
        _propuestaServiceMock = new Mock<IPropuestaCrowdsourcingService>();
        _necesidadServiceMock = new Mock<INecesidadCrowdsourcingService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _validatorMock = new Mock<IValidator<RechazarPropuestaCommand>>();
        _loggerMock = new Mock<ILogger<RechazarPropuestaCommandHandler>>();
        _sut = new RechazarPropuestaCommandHandler(
            _propuestaServiceMock.Object,
            _necesidadServiceMock.Object,
            _artistaServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<RechazarPropuestaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidCommand_RejectsProposal()
    {
        // Arrange
        // NecesidadCrowdsourcingController stores UserId (not Artista entity PK) as ArtistaId
        var userId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(userId: userId.ToString());
        var necesidad = CrowdsourcingTestData.CreateNecesidadAbierta(userId);
        var propuesta = CrowdsourcingTestData.CreatePropuestaPendiente(necesidad.Id);

        var command = new RechazarPropuestaCommand
        {
            PropuestaId = propuesta.Id.Value, UserId = userId.ToString(), Motivo = "No encaja con mi proyecto"
        };

        SetupValidValidation();
        _propuestaServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propuesta);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);
        _necesidadServiceMock.Setup(s => s.GetByIdAsync(propuesta.NecesidadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.EstadoPropuestaNombre.Should().Be("Rechazada");
        _propuestaServiceMock.Verify(s => s.RechazarAsync(
            It.IsAny<PropuestaCrowdsourcingId>(), "No encaja con mi proyecto", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NotOwner_ReturnsForbidden()
    {
        // Arrange
        var propuesta = CrowdsourcingTestData.CreatePropuestaPendiente();
        var necesidad = CrowdsourcingTestData.CreateNecesidadAbierta();

        SetupValidValidation();
        _propuestaServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propuesta);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("other-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAccess.Domain.Model.Artista?)null);
        _necesidadServiceMock.Setup(s => s.GetByIdAsync(propuesta.NecesidadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);

        // Act
        var result = await _sut.Handle(new RechazarPropuestaCommand
        {
            PropuestaId = propuesta.Id.Value, UserId = "other-user"
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_PropuestaNotPendiente_ReturnsConflict()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(userId: userId.ToString());
        var necesidad = CrowdsourcingTestData.CreateNecesidadAbierta(userId);
        var propuesta = CrowdsourcingTestData.CreatePropuesta(necesidad.Id, estadoPropuestaId: EstadoPropuestaConstants.Aceptada);

        SetupValidValidation();
        _propuestaServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propuesta);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);
        _necesidadServiceMock.Setup(s => s.GetByIdAsync(propuesta.NecesidadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);

        // Act
        var result = await _sut.Handle(new RechazarPropuestaCommand
        {
            PropuestaId = propuesta.Id.Value, UserId = userId.ToString()
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_PropuestaNotAcceptable);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        SetupValidValidation();
        _propuestaServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(new RechazarPropuestaCommand
        {
            PropuestaId = Guid.NewGuid(), UserId = "user"
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
