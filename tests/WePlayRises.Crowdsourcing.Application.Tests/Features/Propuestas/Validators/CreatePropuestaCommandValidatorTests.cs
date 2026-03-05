using FluentAssertions;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Validators;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Propuestas.Validators;

public class CreatePropuestaCommandValidatorTests
{
    private readonly Mock<IPropuestaCrowdsourcingService> _propuestaServiceMock;
    private readonly Mock<INecesidadCrowdsourcingService> _necesidadServiceMock;
    private readonly Mock<IPerfilProfesionalService> _perfilServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly CreatePropuestaCommandValidator _sut;

    public CreatePropuestaCommandValidatorTests()
    {
        _propuestaServiceMock = new Mock<IPropuestaCrowdsourcingService>();
        _necesidadServiceMock = new Mock<INecesidadCrowdsourcingService>();
        _perfilServiceMock = new Mock<IPerfilProfesionalService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _sut = new CreatePropuestaCommandValidator(
            _propuestaServiceMock.Object,
            _necesidadServiceMock.Object,
            _perfilServiceMock.Object,
            _artistaServiceMock.Object);
    }

    private CreatePropuestaCommand CreateValidCommand()
    {
        return new CreatePropuestaCommand
        {
            NecesidadId = Guid.NewGuid(),
            UserId = Guid.NewGuid().ToString(),
            PrecioPropuesto = 500m,
            MonedaId = 1,
            DiasEstimados = 15,
            MensajePropuesta = "Tengo experiencia en mezcla profesional y puedo entregar en 15 dias laborables."
        };
    }

    private void SetupValidAsyncRules(CreatePropuestaCommand command)
    {
        var necesidad = CrowdsourcingTestData.CreateNecesidadAbierta();

        _necesidadServiceMock
            .Setup(s => s.GetPublicaByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);

        _perfilServiceMock
            .Setup(s => s.ExistsByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        _propuestaServiceMock
            .Setup(s => s.ExistePropuestaActivaAsync(
                It.IsAny<NecesidadCrowdsourcingId>(), command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }

    [Fact]
    public async Task Validate_ValidCommand_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidAsyncRules(command);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ZeroPrecio_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.PrecioPropuesto = 0;
        SetupValidAsyncRules(command);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Fact]
    public async Task Validate_EmptyMensaje_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.MensajePropuesta = "";
        SetupValidAsyncRules(command);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_MensajeTooShort_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.MensajePropuesta = "Corto";
        SetupValidAsyncRules(command);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_MinLength);
    }

    [Fact]
    public async Task Validate_DiasEstimadosNegative_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.DiasEstimados = 0;
        SetupValidAsyncRules(command);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Fact]
    public async Task Validate_NecesidadNotFound_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();

        _necesidadServiceMock
            .Setup(s => s.GetPublicaByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NecesidadCrowdsourcing?)null);

        _perfilServiceMock
            .Setup(s => s.ExistsByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        _propuestaServiceMock
            .Setup(s => s.ExistePropuestaActivaAsync(
                It.IsAny<NecesidadCrowdsourcingId>(), command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.NotFound_Necesidad);
    }

    [Fact]
    public async Task Validate_AlreadyProposed_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        var necesidad = CrowdsourcingTestData.CreateNecesidadAbierta();

        _necesidadServiceMock
            .Setup(s => s.GetPublicaByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);

        _perfilServiceMock
            .Setup(s => s.ExistsByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        _propuestaServiceMock
            .Setup(s => s.ExistePropuestaActivaAsync(
                It.IsAny<NecesidadCrowdsourcingId>(), command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.BusinessRule_AlreadyProposed);
    }
}
