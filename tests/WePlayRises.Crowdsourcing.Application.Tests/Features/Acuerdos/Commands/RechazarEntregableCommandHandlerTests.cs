using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Commands;

public class RechazarEntregableCommandHandlerTests
{
    private readonly Mock<IAcuerdoCrowdsourcingEntregableService> _entregableServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IValidator<RechazarEntregableCommand>> _validatorMock;
    private readonly Mock<ILogger<RechazarEntregableCommandHandler>> _loggerMock;
    private readonly RechazarEntregableCommandHandler _sut;

    public RechazarEntregableCommandHandlerTests()
    {
        _entregableServiceMock = new Mock<IAcuerdoCrowdsourcingEntregableService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _validatorMock = new Mock<IValidator<RechazarEntregableCommand>>();
        _loggerMock = new Mock<ILogger<RechazarEntregableCommandHandler>>();
        _sut = new RechazarEntregableCommandHandler(
            _entregableServiceMock.Object,
            _artistaServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<RechazarEntregableCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidCommand_RejectsEntregable()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(artistaId, "user-artista");
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(artistaId);
        var entregable = CrowdsourcingTestData.CreateEntregable(acuerdo.Id);
        entregable.Acuerdo = acuerdo;

        SetupValidValidation();
        _entregableServiceMock.Setup(s => s.GetByIdWithAcuerdoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entregable);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-artista", It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);

        // Act
        var result = await _sut.Handle(new RechazarEntregableCommand
        {
            EntregableId = entregable.Id, UserId = "user-artista",
            Comentario = "La calidad del audio necesita mejorar"
        }, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.EstadoEntregableNombre.Should().Be("Rechazado");
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsErrors()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Comentario", "El comentario es obligatorio")
            { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<RechazarEntregableCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _sut.Handle(new RechazarEntregableCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _entregableServiceMock.Verify(s => s.RechazarAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(artistaId, "user-artista");
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(artistaId);
        var entregable = CrowdsourcingTestData.CreateEntregable(acuerdo.Id);
        entregable.Acuerdo = acuerdo;

        SetupValidValidation();
        _entregableServiceMock.Setup(s => s.GetByIdWithAcuerdoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entregable);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-artista", It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);
        _entregableServiceMock.Setup(s => s.RechazarAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(new RechazarEntregableCommand
        {
            EntregableId = entregable.Id, UserId = "user-artista", Comentario = "Needs improvement"
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
