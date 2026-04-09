using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Templates.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Templates.Commands;

public class GenerarNecesidadesDesdeTemplateCommandHandlerTests
{
    private readonly Mock<IPlantillaProyectoService> _plantillaServiceMock;
    private readonly Mock<INecesidadCrowdsourcingService> _necesidadServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<GenerarNecesidadesDesdeTemplateCommand>> _validatorMock;
    private readonly Mock<ILogger<GenerarNecesidadesDesdeTemplateCommandHandler>> _loggerMock;
    private readonly GenerarNecesidadesDesdeTemplateCommandHandler _sut;

    public GenerarNecesidadesDesdeTemplateCommandHandlerTests()
    {
        _plantillaServiceMock = new Mock<IPlantillaProyectoService>();
        _necesidadServiceMock = new Mock<INecesidadCrowdsourcingService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<GenerarNecesidadesDesdeTemplateCommand>>();
        _loggerMock = new Mock<ILogger<GenerarNecesidadesDesdeTemplateCommandHandler>>();
        _sut = new GenerarNecesidadesDesdeTemplateCommandHandler(
            _plantillaServiceMock.Object,
            _necesidadServiceMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GenerarNecesidadesDesdeTemplateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private GenerarNecesidadesDesdeTemplateCommand CreateValidCommand(PlantillaProyecto plantilla)
    {
        return new GenerarNecesidadesDesdeTemplateCommand
        {
            PlantillaId = plantilla.Id.Value,
            ProyectoArtisticoId = Guid.NewGuid(),
            ArtistaId = Guid.NewGuid(),
            UserId = Guid.NewGuid().ToString(),
            NecesidadesSeleccionadas = plantilla.Necesidades.Select(n => new NecesidadSeleccionadaDto
            {
                PlantillaNecesidadId = n.Id.Value,
                PresupuestoMin = 100m,
                PresupuestoMax = 500m,
                MonedaId = 1
            }).ToList()
        };
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesNecesidadesSuccessfully()
    {
        // Arrange
        var plantilla = CrowdsourcingTestData.CreatePlantillaActiva(necesidades: 2);
        var command = CreateValidCommand(plantilla);

        SetupValidValidation();

        _plantillaServiceMock
            .Setup(s => s.GetByIdWithNecesidadesAsync(new PlantillaProyectoId(command.PlantillaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plantilla);

        var createdIds = new List<NecesidadCrowdsourcingId>
        {
            NecesidadCrowdsourcingId.CreateNew(),
            NecesidadCrowdsourcingId.CreateNew()
        };

        _necesidadServiceMock
            .Setup(s => s.CreateManyAsync(It.IsAny<List<NecesidadCrowdsourcing>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdIds);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.NecesidadesCreadas.Should().Be(2);
        result.Data.NecesidadIds.Should().HaveCount(2);
        result.Data.PresupuestoTotalMin.Should().Be(200m);
        result.Data.PresupuestoTotalMax.Should().Be(1000m);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new("PlantillaId", "El ID de la plantilla es obligatorio")
            {
                ErrorCode = ServiceResponseMessageType.Validation_Required
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GenerarNecesidadesDesdeTemplateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var command = new GenerarNecesidadesDesdeTemplateCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Validation_Required);
        _necesidadServiceMock.Verify(s =>
            s.CreateManyAsync(It.IsAny<List<NecesidadCrowdsourcing>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NoUserId_ReturnsUnauthorized()
    {
        // Arrange
        var plantilla = CrowdsourcingTestData.CreatePlantillaActiva();
        var command = CreateValidCommand(plantilla);
        command.UserId = null;

        SetupValidValidation();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Unauthorized);
    }

    [Fact]
    public async Task Handle_PlantillaNotFound_ReturnsNotFound()
    {
        // Arrange
        var plantilla = CrowdsourcingTestData.CreatePlantillaActiva();
        var command = CreateValidCommand(plantilla);

        SetupValidValidation();

        _plantillaServiceMock
            .Setup(s => s.GetByIdWithNecesidadesAsync(It.IsAny<PlantillaProyectoId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlantillaProyecto?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_PlantillaProyecto);
    }

    [Fact]
    public async Task Handle_InvalidNecesidadIds_ReturnsNotFound()
    {
        // Arrange
        var plantilla = CrowdsourcingTestData.CreatePlantillaActiva(necesidades: 1);
        var command = CreateValidCommand(plantilla);
        command.NecesidadesSeleccionadas = new List<NecesidadSeleccionadaDto>
        {
            new()
            {
                PlantillaNecesidadId = Guid.NewGuid(), // ID que no existe en la plantilla
                PresupuestoMin = 100m,
                PresupuestoMax = 500m,
                MonedaId = 1
            }
        };

        SetupValidValidation();

        _plantillaServiceMock
            .Setup(s => s.GetByIdWithNecesidadesAsync(new PlantillaProyectoId(command.PlantillaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plantilla);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_PlantillaNecesidad);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var plantilla = CrowdsourcingTestData.CreatePlantillaActiva();
        var command = CreateValidCommand(plantilla);

        SetupValidValidation();

        _plantillaServiceMock
            .Setup(s => s.GetByIdWithNecesidadesAsync(It.IsAny<PlantillaProyectoId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB connection lost"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_UsesPlantillaDefaultsWhenNoCustomBudget()
    {
        // Arrange
        var plantilla = CrowdsourcingTestData.CreatePlantillaActiva(necesidades: 1);
        var necesidad = plantilla.Necesidades.First();
        necesidad.PrecioMinOrientativo = 300m;
        necesidad.PrecioMaxOrientativo = 800m;

        var command = CreateValidCommand(plantilla);
        command.NecesidadesSeleccionadas = new List<NecesidadSeleccionadaDto>
        {
            new()
            {
                PlantillaNecesidadId = necesidad.Id.Value,
                PresupuestoMin = null,  // No custom budget
                PresupuestoMax = null,
                MonedaId = 1
            }
        };

        SetupValidValidation();

        _plantillaServiceMock
            .Setup(s => s.GetByIdWithNecesidadesAsync(new PlantillaProyectoId(command.PlantillaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plantilla);

        var createdIds = new List<NecesidadCrowdsourcingId> { NecesidadCrowdsourcingId.CreateNew() };
        _necesidadServiceMock
            .Setup(s => s.CreateManyAsync(It.IsAny<List<NecesidadCrowdsourcing>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdIds);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.PresupuestoTotalMin.Should().Be(300m);
        result.Data.PresupuestoTotalMax.Should().Be(800m);
    }
}
