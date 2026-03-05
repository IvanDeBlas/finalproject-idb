using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Commands;

public class AprobarEntregableCommandHandlerTests
{
    private readonly Mock<IAcuerdoCrowdsourcingEntregableService> _entregableServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IValidator<AprobarEntregableCommand>> _validatorMock;
    private readonly Mock<ILogger<AprobarEntregableCommandHandler>> _loggerMock;
    private readonly AprobarEntregableCommandHandler _sut;

    public AprobarEntregableCommandHandlerTests()
    {
        _entregableServiceMock = new Mock<IAcuerdoCrowdsourcingEntregableService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _validatorMock = new Mock<IValidator<AprobarEntregableCommand>>();
        _loggerMock = new Mock<ILogger<AprobarEntregableCommandHandler>>();
        _sut = new AprobarEntregableCommandHandler(
            _entregableServiceMock.Object,
            _artistaServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<AprobarEntregableCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidCommand_ApprovesEntregable()
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
        var result = await _sut.Handle(new AprobarEntregableCommand
        {
            EntregableId = entregable.Id, UserId = "user-artista"
        }, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.EstadoEntregableNombre.Should().Be("Aprobado");
        _entregableServiceMock.Verify(s => s.AprobarAsync(entregable.Id, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EntregableNotEntregado_ReturnsConflict()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(artistaId, "user-artista");
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(artistaId);
        var entregable = CrowdsourcingTestData.CreateEntregable(acuerdo.Id, estadoEntregableId: EstadoEntregableConstants.Aprobado);
        entregable.Acuerdo = acuerdo;

        SetupValidValidation();
        _entregableServiceMock.Setup(s => s.GetByIdWithAcuerdoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entregable);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-artista", It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);

        // Act
        var result = await _sut.Handle(new AprobarEntregableCommand
        {
            EntregableId = entregable.Id, UserId = "user-artista"
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_EntregableNotReviewable);
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
        _entregableServiceMock.Setup(s => s.AprobarAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(new AprobarEntregableCommand
        {
            EntregableId = entregable.Id, UserId = "user-artista"
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
