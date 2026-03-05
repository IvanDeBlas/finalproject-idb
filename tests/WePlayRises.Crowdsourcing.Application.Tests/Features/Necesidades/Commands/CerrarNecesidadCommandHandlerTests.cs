using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Necesidades.Commands;

public class CerrarNecesidadCommandHandlerTests
{
    private readonly Mock<INecesidadCrowdsourcingService> _serviceMock;
    private readonly Mock<IValidator<CerrarNecesidadCommand>> _validatorMock;
    private readonly Mock<ILogger<CerrarNecesidadCommandHandler>> _loggerMock;
    private readonly CerrarNecesidadCommandHandler _sut;

    public CerrarNecesidadCommandHandlerTests()
    {
        _serviceMock = new Mock<INecesidadCrowdsourcingService>();
        _validatorMock = new Mock<IValidator<CerrarNecesidadCommand>>();
        _loggerMock = new Mock<ILogger<CerrarNecesidadCommandHandler>>();
        _sut = new CerrarNecesidadCommandHandler(
            _serviceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CerrarNecesidadCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private CerrarNecesidadCommand CreateValidCommand(Guid? id = null)
    {
        return new CerrarNecesidadCommand
        {
            Id = id ?? Guid.NewGuid(),
            Motivo = "Ya no necesitamos este servicio",
            ArtistaId = Guid.NewGuid()
        };
    }

    [Fact]
    public async Task Handle_ValidCommand_ClosesNecesidadSuccessfully()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation();

        _serviceMock
            .Setup(s => s.CerrarAsync(
                It.IsAny<NecesidadCrowdsourcingId>(),
                command.Motivo,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EstadoNecesidadNombre.Should().Be("Cerrada");
        result.Data.PropuestasRechazadas.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ValidCommand_WithPendingProposals_RejectsThemAll()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation();

        _serviceMock
            .Setup(s => s.CerrarAsync(
                It.IsAny<NecesidadCrowdsourcingId>(),
                command.Motivo,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.PropuestasRechazadas.Should().Be(3);
        result.Messages.Should().ContainSingle(m =>
            m.Message!.Contains("3 propuestas pendientes"));
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new("", "Solo se pueden cerrar necesidades en estado Abierta o En Progreso que te pertenezcan")
            {
                ErrorCode = ServiceResponseMessageType.BusinessRule_NecesidadNotCloseable
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CerrarNecesidadCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var command = CreateValidCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_NecesidadNotCloseable);
        _serviceMock.Verify(s =>
            s.CerrarAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation();

        _serviceMock
            .Setup(s => s.CerrarAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Transaction failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
