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

public class UpdateNecesidadCommandHandlerTests
{
    private readonly Mock<INecesidadCrowdsourcingService> _serviceMock;
    private readonly Mock<IValidator<UpdateNecesidadCommand>> _validatorMock;
    private readonly Mock<ILogger<UpdateNecesidadCommandHandler>> _loggerMock;
    private readonly UpdateNecesidadCommandHandler _sut;

    public UpdateNecesidadCommandHandlerTests()
    {
        _serviceMock = new Mock<INecesidadCrowdsourcingService>();
        _validatorMock = new Mock<IValidator<UpdateNecesidadCommand>>();
        _loggerMock = new Mock<ILogger<UpdateNecesidadCommandHandler>>();
        _sut = new UpdateNecesidadCommandHandler(
            _serviceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateNecesidadCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private UpdateNecesidadCommand CreateValidCommand(Guid? necesidadId = null, Guid? artistaId = null)
    {
        return new UpdateNecesidadCommand
        {
            Id = necesidadId ?? Guid.NewGuid(),
            Titulo = "Titulo actualizado para prueba",
            Descripcion = "Descripcion actualizada",
            ModalidadTrabajoId = 2,
            PresupuestoMin = 200m,
            PresupuestoMax = 900m,
            MonedaId = 1,
            ArtistaId = artistaId ?? Guid.NewGuid()
        };
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesNecesidadSuccessfully()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var necesidadId = Guid.NewGuid();
        var necesidad = CrowdsourcingTestData.CreateNecesidadAbierta(artistaId);
        necesidad.Id = new NecesidadCrowdsourcingId(necesidadId);

        var command = CreateValidCommand(necesidadId, artistaId);
        SetupValidValidation();

        _serviceMock
            .Setup(s => s.GetByIdAsync(new NecesidadCrowdsourcingId(necesidadId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<NecesidadCrowdsourcing>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Titulo.Should().Be("Titulo actualizado para prueba");
        result.Data.EstadoNecesidadId.Should().Be(1);
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
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateNecesidadCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var command = CreateValidCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Validation_Required);
        _serviceMock.Verify(s =>
            s.UpdateAsync(It.IsAny<NecesidadCrowdsourcing>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NecesidadNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation();

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NecesidadCrowdsourcing?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Necesidad);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation();

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
