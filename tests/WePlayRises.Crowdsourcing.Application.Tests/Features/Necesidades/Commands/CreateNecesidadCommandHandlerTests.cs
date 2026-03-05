using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Necesidades.Commands;

public class CreateNecesidadCommandHandlerTests
{
    private readonly Mock<INecesidadCrowdsourcingService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateNecesidadCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateNecesidadCommandHandler>> _loggerMock;
    private readonly CreateNecesidadCommandHandler _sut;

    public CreateNecesidadCommandHandlerTests()
    {
        _serviceMock = new Mock<INecesidadCrowdsourcingService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreateNecesidadCommand>>();
        _loggerMock = new Mock<ILogger<CreateNecesidadCommandHandler>>();
        _sut = new CreateNecesidadCommandHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateNecesidadCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private CreateNecesidadCommand CreateValidCommand()
    {
        return new CreateNecesidadCommand
        {
            Titulo = "Mezcla de pistas para EP indie",
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
    public async Task Handle_ValidCommand_CreatesNecesidadSuccessfully()
    {
        // Arrange
        var command = CreateValidCommand();
        var createdId = NecesidadCrowdsourcingId.CreateNew();

        SetupValidValidation();

        _mapperMock
            .Setup(m => m.Map<NecesidadCrowdsourcing>(command))
            .Returns(new NecesidadCrowdsourcing { Titulo = command.Titulo });

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<NecesidadCrowdsourcing>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Titulo.Should().Be(command.Titulo);
        result.Data.EstadoNecesidadId.Should().Be(1);
        result.Data.EstadoNecesidadNombre.Should().Be("Abierta");
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new("Titulo", "El titulo es obligatorio")
            {
                ErrorCode = ServiceResponseMessageType.Validation_Required
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateNecesidadCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var command = new CreateNecesidadCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Validation_Required);
        _serviceMock.Verify(s =>
            s.CreateAsync(It.IsAny<NecesidadCrowdsourcing>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation();

        _mapperMock
            .Setup(m => m.Map<NecesidadCrowdsourcing>(command))
            .Returns(new NecesidadCrowdsourcing { Titulo = command.Titulo });

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<NecesidadCrowdsourcing>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB connection lost"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_ValidCommand_SetsEstadoToAbierta()
    {
        // Arrange
        var command = CreateValidCommand();
        NecesidadCrowdsourcing? capturedEntity = null;

        SetupValidValidation();

        _mapperMock
            .Setup(m => m.Map<NecesidadCrowdsourcing>(command))
            .Returns(new NecesidadCrowdsourcing { Titulo = command.Titulo });

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<NecesidadCrowdsourcing>(), It.IsAny<CancellationToken>()))
            .Callback<NecesidadCrowdsourcing, CancellationToken>((e, _) => capturedEntity = e)
            .ReturnsAsync(NecesidadCrowdsourcingId.CreateNew());

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        capturedEntity.Should().NotBeNull();
        capturedEntity!.EstadoNecesidadId.Should().Be(1);
        capturedEntity.ArtistaId.Value.Should().Be(command.ArtistaId);
        capturedEntity.ProyectoArtisticoId.Value.Should().Be(command.ProyectoArtisticoId);
        capturedEntity.FechaCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
