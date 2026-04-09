using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;
using PromoProgramaEntity = WePlayRises.Crowdpromotion.Domain.Model.PromoPrograma;
using Xunit;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.PromoTarea.Commands;

public class ValidarTareaCommandHandlerTests
{
    private readonly Mock<IPromoTareaService> _serviceMock;
    private readonly Mock<IValidator<ValidarTareaCommand>> _validatorMock;
    private readonly Mock<ILogger<ValidarTareaCommandHandler>> _loggerMock;
    private readonly ValidarTareaCommandHandler _sut;

    public ValidarTareaCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromoTareaService>();
        _validatorMock = new Mock<IValidator<ValidarTareaCommand>>();
        _loggerMock = new Mock<ILogger<ValidarTareaCommandHandler>>();

        _sut = new ValidarTareaCommandHandler(
            _serviceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidarTareaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupHappyPath()
    {
        SetupValidValidation();

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PromoTareaTestData.DefaultArtistaId);

        var programa = PromoTareaTestData.CreateValidPrograma();
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        var registro = PromoTareaTestData.CreateValidTareaPromotor(estadoTareaId: 2);
        _serviceMock
            .Setup(s => s.GetTareaPromotorByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registro);

        var responseDto = new ValidarTareaResponseDto
        {
            TareaPromotorId = registro.Id,
            EstadoTareaId = 3,
            EstadoTareaNombre = "Validada",
            RecompensaAcreditada = 100m,
            MonedaNombre = "Puntos",
            PuntosAcreditados = 100
        };
        _serviceMock
            .Setup(s => s.ValidarTareaAsync(
                It.IsAny<PromoTareaPromotor>(), It.IsAny<PromoProgramaId>(),
                It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        SetupHappyPath();
        var command = PromoTareaTestData.CreateValidValidarCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EstadoTareaId.Should().Be(3);
        result.Data.EstadoTareaNombre.Should().Be("Validada");

        _serviceMock.Verify(
            s => s.ValidarTareaAsync(
                It.IsAny<PromoTareaPromotor>(), It.IsAny<PromoProgramaId>(),
                It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = PromoTareaTestData.CreateValidValidarCommand();
        var failures = new List<ValidationFailure>
        {
            new("TareaPromotorId", "Required") { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };
        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _serviceMock.Verify(
            s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupValidValidation();
        var command = PromoTareaTestData.CreateValidValidarCommand();
        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArtistaId?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Artista);
    }

    [Fact]
    public async Task Handle_NotProgramOwner_ReturnsForbidden()
    {
        // Arrange
        SetupValidValidation();
        var command = PromoTareaTestData.CreateValidValidarCommand();
        var differentArtistaId = new ArtistaId(Guid.NewGuid());

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(differentArtistaId);

        var programa = PromoTareaTestData.CreateValidPrograma();
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);
    }

    [Fact]
    public async Task Handle_TareaPromotorNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupValidValidation();
        var command = PromoTareaTestData.CreateValidValidarCommand();
        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PromoTareaTestData.DefaultArtistaId);
        var programa = PromoTareaTestData.CreateValidPrograma();
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);
        _serviceMock
            .Setup(s => s.GetTareaPromotorByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromoTareaPromotor?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_PromoTareaPromotor);
    }

    [Fact]
    public async Task Handle_EstadoNoCompletada_ReturnsBadRequest()
    {
        // Arrange
        SetupHappyPath();
        var command = PromoTareaTestData.CreateValidValidarCommand();
        var registroValidado = PromoTareaTestData.CreateValidTareaPromotor(estadoTareaId: 3);

        _serviceMock
            .Setup(s => s.GetTareaPromotorByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registroValidado);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_CompletadoEstadoInvalido);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        SetupHappyPath();
        var command = PromoTareaTestData.CreateValidValidarCommand();
        _serviceMock
            .Setup(s => s.ValidarTareaAsync(
                It.IsAny<PromoTareaPromotor>(), It.IsAny<PromoProgramaId>(),
                It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Transaction failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
