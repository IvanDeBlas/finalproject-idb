using AutoMapper;
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
using WePlayRises.Crowdpromotion.Application.Mapping;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;
using PromoTareaEntity = WePlayRises.Crowdpromotion.Domain.Model.PromoTarea;
using PromoProgramaEntity = WePlayRises.Crowdpromotion.Domain.Model.PromoPrograma;
using Xunit;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.PromoTarea.Commands;

public class CompletarTareaCommandHandlerTests
{
    private readonly Mock<IPromoTareaService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<CompletarTareaCommand>> _validatorMock;
    private readonly Mock<ILogger<CompletarTareaCommandHandler>> _loggerMock;
    private readonly CompletarTareaCommandHandler _sut;

    public CompletarTareaCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromoTareaService>();
        _validatorMock = new Mock<IValidator<CompletarTareaCommand>>();
        _loggerMock = new Mock<ILogger<CompletarTareaCommandHandler>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PromoTareaProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _sut = new CompletarTareaCommandHandler(
            _serviceMock.Object,
            _mapper,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CompletarTareaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupHappyPath()
    {
        SetupValidValidation();

        var promotor = PromoTareaTestData.CreateValidPromotor();
        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        var programa = PromoTareaTestData.CreateValidPrograma();
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        var inscripcion = PromoTareaTestData.CreateValidInscripcion();
        _serviceMock
            .Setup(s => s.GetInscripcionAprobadaAsync(
                It.IsAny<PromotorId>(), It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(inscripcion);

        var tarea = PromoTareaTestData.CreateValidTarea();
        _serviceMock
            .Setup(s => s.GetTareaByIdAndProgramaAsync(
                It.IsAny<Guid>(), It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tarea);

        _serviceMock
            .Setup(s => s.ContarCompletadosActivosAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _serviceMock
            .Setup(s => s.GetUltimoCompletadoAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromoTareaPromotor?)null);

        var responseDto = new CompletarTareaResponseDto
        {
            TareaPromotorId = Guid.NewGuid(),
            EstadoTareaId = 2,
            EstadoTareaNombre = "Completada",
            VecesCompletada = 1,
            FechaUltimaCompletada = DateTime.UtcNow
        };
        _serviceMock
            .Setup(s => s.CompletarTareaAsync(
                It.IsAny<PromoTareaPromotor>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((responseDto, 1));
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        SetupHappyPath();
        var command = PromoTareaTestData.CreateValidCompletarCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EstadoTareaId.Should().Be(2);

        _serviceMock.Verify(
            s => s.CompletarTareaAsync(
                It.IsAny<PromoTareaPromotor>(), false, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = PromoTareaTestData.CreateValidCompletarCommand();
        var failures = new List<ValidationFailure>
        {
            new("UrlPruebaCompletado", "La URL es obligatoria")
                { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };
        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().NotBeEmpty();

        _serviceMock.Verify(
            s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_PromotorNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupValidValidation();
        var command = PromoTareaTestData.CreateValidCompletarCommand();
        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Promotor?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Promotor);
    }

    [Fact]
    public async Task Handle_ProgramaNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupValidValidation();
        var command = PromoTareaTestData.CreateValidCompletarCommand();
        var promotor = PromoTareaTestData.CreateValidPromotor();

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromoProgramaEntity?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_PromoPrograma);
    }

    [Fact]
    public async Task Handle_ProgramaInactivo_ReturnsBadRequest()
    {
        // Arrange
        SetupValidValidation();
        var command = PromoTareaTestData.CreateValidCompletarCommand();
        var promotor = PromoTareaTestData.CreateValidPromotor();
        var programa = PromoTareaTestData.CreateValidPrograma(esActivo: false);

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_ProgramaInactivo);
    }

    [Fact]
    public async Task Handle_NoInscripcionAprobada_ReturnsForbidden()
    {
        // Arrange
        SetupValidValidation();
        var command = PromoTareaTestData.CreateValidCompletarCommand();
        var promotor = PromoTareaTestData.CreateValidPromotor();
        var programa = PromoTareaTestData.CreateValidPrograma();

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);
        _serviceMock
            .Setup(s => s.GetInscripcionAprobadaAsync(
                It.IsAny<PromotorId>(), It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromoProgramaPromotor?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_TareaNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupValidValidation();
        var command = PromoTareaTestData.CreateValidCompletarCommand();
        var promotor = PromoTareaTestData.CreateValidPromotor();
        var programa = PromoTareaTestData.CreateValidPrograma();
        var inscripcion = PromoTareaTestData.CreateValidInscripcion();

        _serviceMock
            .Setup(s => s.GetPromotorByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);
        _serviceMock
            .Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);
        _serviceMock
            .Setup(s => s.GetInscripcionAprobadaAsync(
                It.IsAny<PromotorId>(), It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(inscripcion);
        _serviceMock
            .Setup(s => s.GetTareaByIdAndProgramaAsync(
                It.IsAny<Guid>(), It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromoTareaEntity?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_PromoTarea);
    }

    [Fact]
    public async Task Handle_TareaNoRepetible_AlreadyCompleted_ReturnsBadRequest()
    {
        // Arrange
        SetupHappyPath();
        var command = PromoTareaTestData.CreateValidCompletarCommand();
        var tarea = PromoTareaTestData.CreateValidTarea(esRepetible: false);

        _serviceMock
            .Setup(s => s.GetTareaByIdAndProgramaAsync(
                It.IsAny<Guid>(), It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tarea);
        _serviceMock
            .Setup(s => s.ContarCompletadosActivosAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_TareaNoRepetible);
    }

    [Fact]
    public async Task Handle_MaxRepeticionesAlcanzado_ReturnsBadRequest()
    {
        // Arrange
        SetupHappyPath();
        var command = PromoTareaTestData.CreateValidCompletarCommand();
        var tarea = PromoTareaTestData.CreateValidTarea(esRepetible: true, maxRepeticiones: 3);

        _serviceMock
            .Setup(s => s.GetTareaByIdAndProgramaAsync(
                It.IsAny<Guid>(), It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tarea);
        _serviceMock
            .Setup(s => s.ContarCompletadosActivosAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_MaxRepeticionesAlcanzado);
    }

    [Fact]
    public async Task Handle_RejectedRecord_UpdatesExisting()
    {
        // Arrange
        SetupHappyPath();
        var command = PromoTareaTestData.CreateValidCompletarCommand();
        var rejectedRecord = PromoTareaTestData.CreateValidTareaPromotor(estadoTareaId: 4);

        _serviceMock
            .Setup(s => s.GetUltimoCompletadoAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(rejectedRecord);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(
            s => s.CompletarTareaAsync(
                It.IsAny<PromoTareaPromotor>(), true, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        SetupHappyPath();
        var command = PromoTareaTestData.CreateValidCompletarCommand();
        _serviceMock
            .Setup(s => s.CompletarTareaAsync(
                It.IsAny<PromoTareaPromotor>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
