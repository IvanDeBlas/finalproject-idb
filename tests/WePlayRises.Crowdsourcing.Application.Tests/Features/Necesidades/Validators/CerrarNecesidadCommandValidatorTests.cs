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

public class CerrarNecesidadCommandValidatorTests
{
    private readonly Mock<INecesidadCrowdsourcingService> _serviceMock;
    private readonly CerrarNecesidadCommandValidator _sut;

    public CerrarNecesidadCommandValidatorTests()
    {
        _serviceMock = new Mock<INecesidadCrowdsourcingService>();
        _sut = new CerrarNecesidadCommandValidator(_serviceMock.Object);
    }

    private CerrarNecesidadCommand CreateValidCommand(Guid? id = null, Guid? artistaId = null)
    {
        return new CerrarNecesidadCommand
        {
            Id = id ?? Guid.NewGuid(),
            Motivo = "Ya no necesitamos este servicio",
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
    public async Task Validate_ValidCommand_NecesidadEnProgreso_ReturnsValid()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var necesidadId = Guid.NewGuid();
        var command = CreateValidCommand(necesidadId, artistaId);
        SetupNecesidadExists(necesidadId, artistaId, estado: 2); // En Progreso

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_NecesidadCerrada_ReturnsError()
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
            e.ErrorCode == ServiceResponseMessageType.BusinessRule_NecesidadNotCloseable);
    }

    [Fact]
    public async Task Validate_NecesidadNotFound_ReturnsError()
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
            e.ErrorCode == ServiceResponseMessageType.BusinessRule_NecesidadNotCloseable);
    }

    [Fact]
    public async Task Validate_DifferentArtista_ReturnsError()
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
            e.ErrorCode == ServiceResponseMessageType.BusinessRule_NecesidadNotCloseable);
    }

    [Fact]
    public async Task Validate_MotivoTooLong_ReturnsError()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var necesidadId = Guid.NewGuid();
        var command = CreateValidCommand(necesidadId, artistaId);
        command.Motivo = new string('A', 501);
        SetupNecesidadExists(necesidadId, artistaId);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_NullMotivo_IsValid()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var necesidadId = Guid.NewGuid();
        var command = CreateValidCommand(necesidadId, artistaId);
        command.Motivo = null;
        SetupNecesidadExists(necesidadId, artistaId);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
