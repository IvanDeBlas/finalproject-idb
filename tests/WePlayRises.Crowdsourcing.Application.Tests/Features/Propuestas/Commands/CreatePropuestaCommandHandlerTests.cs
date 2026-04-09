using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Propuestas.Commands;

public class CreatePropuestaCommandHandlerTests
{
    private readonly Mock<IPropuestaCrowdsourcingService> _serviceMock;
    private readonly Mock<INecesidadCrowdsourcingService> _necesidadServiceMock;
    private readonly Mock<IPerfilProfesionalService> _perfilServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreatePropuestaCommand>> _validatorMock;
    private readonly Mock<ILogger<CreatePropuestaCommandHandler>> _loggerMock;
    private readonly CreatePropuestaCommandHandler _sut;

    public CreatePropuestaCommandHandlerTests()
    {
        _serviceMock = new Mock<IPropuestaCrowdsourcingService>();
        _necesidadServiceMock = new Mock<INecesidadCrowdsourcingService>();
        _perfilServiceMock = new Mock<IPerfilProfesionalService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreatePropuestaCommand>>();
        _loggerMock = new Mock<ILogger<CreatePropuestaCommandHandler>>();
        _sut = new CreatePropuestaCommandHandler(
            _serviceMock.Object,
            _necesidadServiceMock.Object,
            _perfilServiceMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreatePropuestaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
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
            MensajePropuesta = "Tengo experiencia en mezcla profesional y puedo entregar en 15 dias."
        };
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesPropuestaSuccessfully()
    {
        // Arrange
        var command = CreateValidCommand();
        var createdId = PropuestaCrowdsourcingId.CreateNew();
        var perfilId = new PerfilProfesionalId(Guid.NewGuid());
        var necesidad = CrowdsourcingTestData.CreateNecesidadAbierta();

        SetupValidValidation();

        _perfilServiceMock
            .Setup(s => s.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PerfilProfesional { Id = perfilId, UserId = command.UserId });

        _mapperMock
            .Setup(m => m.Map<PropuestaCrowdsourcing>(command))
            .Returns(new PropuestaCrowdsourcing { PrecioPropuesto = command.PrecioPropuesto });

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<PropuestaCrowdsourcing>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdId);

        _necesidadServiceMock
            .Setup(s => s.GetPublicaByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(createdId.Value);
        result.Data.PrecioPropuesto.Should().Be(500m);
        result.Data.EstadoPropuestaNombre.Should().Be("Pendiente");
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new("PrecioPropuesto", "El precio propuesto debe ser mayor a 0")
            {
                ErrorCode = ServiceResponseMessageType.Validation_InvalidRange
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreatePropuestaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var command = new CreatePropuestaCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Validation_InvalidRange);
        _serviceMock.Verify(s =>
            s.CreateAsync(It.IsAny<PropuestaCrowdsourcing>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NoPerfilProfesional_ReturnsBusinessError()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation();

        _perfilServiceMock
            .Setup(s => s.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PerfilProfesional?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_NoProfessionalProfile);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = CreateValidCommand();
        var perfilId = new PerfilProfesionalId(Guid.NewGuid());
        SetupValidValidation();

        _perfilServiceMock
            .Setup(s => s.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PerfilProfesional { Id = perfilId, UserId = command.UserId });

        _mapperMock
            .Setup(m => m.Map<PropuestaCrowdsourcing>(command))
            .Returns(new PropuestaCrowdsourcing { PrecioPropuesto = command.PrecioPropuesto });

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<PropuestaCrowdsourcing>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_ValidCommand_SetsEstadoPendiente()
    {
        // Arrange
        var command = CreateValidCommand();
        PropuestaCrowdsourcing? capturedEntity = null;
        var perfilId = new PerfilProfesionalId(Guid.NewGuid());

        SetupValidValidation();

        _perfilServiceMock
            .Setup(s => s.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PerfilProfesional { Id = perfilId, UserId = command.UserId });

        _mapperMock
            .Setup(m => m.Map<PropuestaCrowdsourcing>(command))
            .Returns(new PropuestaCrowdsourcing { PrecioPropuesto = command.PrecioPropuesto });

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<PropuestaCrowdsourcing>(), It.IsAny<CancellationToken>()))
            .Callback<PropuestaCrowdsourcing, CancellationToken>((e, _) => capturedEntity = e)
            .ReturnsAsync(PropuestaCrowdsourcingId.CreateNew());

        _necesidadServiceMock
            .Setup(s => s.GetPublicaByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CrowdsourcingTestData.CreateNecesidadAbierta());

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        capturedEntity.Should().NotBeNull();
        capturedEntity!.EstadoPropuestaId.Should().Be(EstadoPropuestaConstants.Pendiente);
        capturedEntity.UserId.Should().Be(command.UserId);
        capturedEntity.PerfilProfesionalId.Should().Be(perfilId);
        capturedEntity.FechaCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
