using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Templates.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Templates.Validators;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Templates.Validators;

public class GenerarNecesidadesDesdeTemplateValidatorTests
{
    private readonly Mock<IPlantillaProyectoService> _plantillaServiceMock;
    private readonly GenerarNecesidadesDesdeTemplateValidator _sut;

    public GenerarNecesidadesDesdeTemplateValidatorTests()
    {
        _plantillaServiceMock = new Mock<IPlantillaProyectoService>();
        _sut = new GenerarNecesidadesDesdeTemplateValidator(
            _plantillaServiceMock.Object);
    }

    private GenerarNecesidadesDesdeTemplateCommand CreateValidCommand()
    {
        var plantillaId = Guid.NewGuid();

        // Setup plantilla activa para validacion async
        var plantilla = CrowdsourcingTestData.CreatePlantillaActiva(plantillaId);
        _plantillaServiceMock
            .Setup(s => s.GetByIdAsync(new PlantillaProyectoId(plantillaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plantilla);

        return new GenerarNecesidadesDesdeTemplateCommand
        {
            PlantillaId = plantillaId,
            ProyectoArtisticoId = Guid.NewGuid(),
            ArtistaId = Guid.NewGuid(),
            NecesidadesSeleccionadas = new List<NecesidadSeleccionadaDto>
            {
                new()
                {
                    PlantillaNecesidadId = Guid.NewGuid(),
                    PresupuestoMin = 100m,
                    PresupuestoMax = 500m,
                    MonedaId = 1
                }
            }
        };
    }

    [Fact]
    public async Task Validate_ValidCommand_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyPlantillaId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.PlantillaId = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.PlantillaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyProyectoArtisticoId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ProyectoArtisticoId = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ProyectoArtisticoId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyArtistaId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ArtistaId = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ArtistaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyNecesidadesSeleccionadas_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.NecesidadesSeleccionadas = new List<NecesidadSeleccionadaDto>();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.NecesidadesSeleccionadas)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_NecesidadWithEmptyId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.NecesidadesSeleccionadas = new List<NecesidadSeleccionadaDto>
        {
            new()
            {
                PlantillaNecesidadId = Guid.Empty,
                PresupuestoMin = 100m,
                PresupuestoMax = 500m,
                MonedaId = 1
            }
        };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_Required &&
            e.PropertyName.Contains("PlantillaNecesidadId"));
    }

    [Fact]
    public async Task Validate_NegativePresupuestoMin_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.NecesidadesSeleccionadas[0].PresupuestoMin = -100m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_InvalidRange &&
            e.PropertyName.Contains("PresupuestoMin"));
    }

    [Fact]
    public async Task Validate_PresupuestoMaxLessThanMin_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.NecesidadesSeleccionadas[0].PresupuestoMin = 500m;
        command.NecesidadesSeleccionadas[0].PresupuestoMax = 100m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_InvalidRange &&
            e.PropertyName.Contains("PresupuestoMax"));
    }

    [Fact]
    public async Task Validate_ZeroMonedaId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.NecesidadesSeleccionadas[0].MonedaId = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_Required &&
            e.PropertyName.Contains("MonedaId"));
    }

    [Fact]
    public async Task Validate_PlantillaNotFound_ReturnsError()
    {
        // Arrange
        var plantillaId = Guid.NewGuid();
        _plantillaServiceMock
            .Setup(s => s.GetByIdAsync(new PlantillaProyectoId(plantillaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlantillaProyecto?)null);

        var command = new GenerarNecesidadesDesdeTemplateCommand
        {
            PlantillaId = plantillaId,
            ProyectoArtisticoId = Guid.NewGuid(),
            ArtistaId = Guid.NewGuid(),
            NecesidadesSeleccionadas = new List<NecesidadSeleccionadaDto>
            {
                new()
                {
                    PlantillaNecesidadId = Guid.NewGuid(),
                    PresupuestoMin = 100m,
                    PresupuestoMax = 500m,
                    MonedaId = 1
                }
            }
        };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.PlantillaId)
            .WithErrorCode(ServiceResponseMessageType.NotFound_PlantillaProyecto);
    }

    [Fact]
    public async Task Validate_PlantillaInactiva_ReturnsError()
    {
        // Arrange
        var plantillaId = Guid.NewGuid();
        var plantillaInactiva = CrowdsourcingTestData.CreatePlantillaInactiva(plantillaId);

        _plantillaServiceMock
            .Setup(s => s.GetByIdAsync(new PlantillaProyectoId(plantillaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plantillaInactiva);

        var command = new GenerarNecesidadesDesdeTemplateCommand
        {
            PlantillaId = plantillaId,
            ProyectoArtisticoId = Guid.NewGuid(),
            ArtistaId = Guid.NewGuid(),
            NecesidadesSeleccionadas = new List<NecesidadSeleccionadaDto>
            {
                new()
                {
                    PlantillaNecesidadId = Guid.NewGuid(),
                    PresupuestoMin = 100m,
                    PresupuestoMax = 500m,
                    MonedaId = 1
                }
            }
        };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.PlantillaId)
            .WithErrorCode(ServiceResponseMessageType.NotFound_PlantillaProyecto);
    }
}
