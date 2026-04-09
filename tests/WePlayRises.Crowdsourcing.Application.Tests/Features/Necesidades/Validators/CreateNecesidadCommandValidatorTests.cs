using FluentAssertions;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Validators;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Necesidades.Validators;

public class CreateNecesidadCommandValidatorTests
{
    private readonly CreateNecesidadCommandValidator _sut;

    public CreateNecesidadCommandValidatorTests()
    {
        _sut = new CreateNecesidadCommandValidator();
    }

    private CreateNecesidadCommand CreateValidCommand()
    {
        return new CreateNecesidadCommand
        {
            Titulo = "Mezcla profesional de canciones",
            Descripcion = "Buscamos ingeniero de mezcla",
            TipoNecesidadId = 1,
            ModalidadTrabajoId = 2,
            PresupuestoMin = 100m,
            PresupuestoMax = 500m,
            MonedaId = 1,
            ProyectoArtisticoId = Guid.NewGuid(),
            ArtistaId = Guid.NewGuid()
        };
    }

    [Fact]
    public async Task Validate_ValidCommand_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyTitulo_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Titulo = "";

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_TituloTooShort_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Titulo = "Abc";

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_MinLength);
    }

    [Fact]
    public async Task Validate_TituloTooLong_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Titulo = new string('A', 201);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_EmptyTipoNecesidadId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TipoNecesidadId = 0;

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_Required &&
            e.PropertyName == "TipoNecesidadId");
    }

    [Fact]
    public async Task Validate_EmptyModalidadTrabajoId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ModalidadTrabajoId = 0;

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_Required &&
            e.PropertyName == "ModalidadTrabajoId");
    }

    [Fact]
    public async Task Validate_PresupuestoMaxLessThanMin_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.PresupuestoMin = 800m;
        command.PresupuestoMax = 150m;

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Fact]
    public async Task Validate_PresupuestoWithoutMoneda_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.PresupuestoMin = 100m;
        command.MonedaId = null;

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_Required &&
            e.PropertyName == "MonedaId");
    }

    [Fact]
    public async Task Validate_PresencialWithoutUbicacion_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ModalidadTrabajoId = 1; // Presencial
        command.UbicacionCiudad = null;
        command.UbicacionPais = null;

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "UbicacionCiudad");
        result.Errors.Should().Contain(e =>
            e.PropertyName == "UbicacionPais");
    }

    [Fact]
    public async Task Validate_EmptyProyectoArtisticoId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ProyectoArtisticoId = Guid.Empty;

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_Required &&
            e.PropertyName == "ProyectoArtisticoId");
    }

    [Fact]
    public async Task Validate_EmptyArtistaId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ArtistaId = Guid.Empty;

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_Required &&
            e.PropertyName == "ArtistaId");
    }
}
