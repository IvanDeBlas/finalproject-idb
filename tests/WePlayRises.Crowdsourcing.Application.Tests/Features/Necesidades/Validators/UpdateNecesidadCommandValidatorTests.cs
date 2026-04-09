using FluentAssertions;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Validators;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Necesidades.Validators;

public class UpdateNecesidadCommandValidatorTests
{
    private readonly Mock<INecesidadCrowdsourcingService> _serviceMock;
    private readonly UpdateNecesidadCommandValidator _sut;

    public UpdateNecesidadCommandValidatorTests()
    {
        _serviceMock = new Mock<INecesidadCrowdsourcingService>();
        _sut = new UpdateNecesidadCommandValidator(_serviceMock.Object);
    }

    private UpdateNecesidadCommand CreateValidCommand(Guid? id = null, Guid? artistaId = null)
    {
        return new UpdateNecesidadCommand
        {
            Id = id ?? Guid.NewGuid(),
            Titulo = "Titulo valido actualizado",
            Descripcion = "Descripcion actualizada",
            ModalidadTrabajoId = 2,
            PresupuestoMin = 200m,
            PresupuestoMax = 900m,
            MonedaId = 1,
            ArtistaId = artistaId ?? Guid.NewGuid()
        };
    }

    private void SetupNecesidadExists(Guid necesidadId, Guid artistaId, int estado = 1)
    {
        var necesidad = CrowdsourcingTestData.CreateNecesidad(
            id: necesidadId,
            artistaId: artistaId,
            estadoNecesidadId: estado);

        _serviceMock
            .Setup(s => s.GetByIdAsync(new NecesidadCrowdsourcingId(necesidadId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);
    }

    [Fact]
    public async Task Validate_ValidCommand_NecesidadAbierta_ReturnsValid()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var necesidadId = Guid.NewGuid();
        var command = CreateValidCommand(necesidadId, artistaId);
        SetupNecesidadExists(necesidadId, artistaId, estado: 1);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyTitulo_ReturnsError()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var necesidadId = Guid.NewGuid();
        var command = CreateValidCommand(necesidadId, artistaId);
        command.Titulo = "";
        SetupNecesidadExists(necesidadId, artistaId);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_Required &&
            e.PropertyName == "Titulo");
    }

    [Fact]
    public async Task Validate_NecesidadCerrada_ReturnsBusinessRuleError()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var necesidadId = Guid.NewGuid();
        var command = CreateValidCommand(necesidadId, artistaId);
        SetupNecesidadExists(necesidadId, artistaId, estado: 3); // Cerrada

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.BusinessRule_NecesidadNotEditable);
    }

    [Fact]
    public async Task Validate_NecesidadNotFound_ReturnsBusinessRuleError()
    {
        // Arrange
        var command = CreateValidCommand();

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NecesidadCrowdsourcing?)null);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.BusinessRule_NecesidadNotEditable);
    }

    [Fact]
    public async Task Validate_DifferentArtista_ReturnsBusinessRuleError()
    {
        // Arrange
        var ownerArtistaId = Guid.NewGuid();
        var requestingArtistaId = Guid.NewGuid();
        var necesidadId = Guid.NewGuid();
        var command = CreateValidCommand(necesidadId, requestingArtistaId);
        SetupNecesidadExists(necesidadId, ownerArtistaId, estado: 1);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.BusinessRule_NecesidadNotEditable);
    }

    [Fact]
    public async Task Validate_PresupuestoMaxLessThanMin_ReturnsError()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var necesidadId = Guid.NewGuid();
        var command = CreateValidCommand(necesidadId, artistaId);
        command.PresupuestoMin = 800m;
        command.PresupuestoMax = 150m;
        SetupNecesidadExists(necesidadId, artistaId);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_InvalidRange);
    }
}
