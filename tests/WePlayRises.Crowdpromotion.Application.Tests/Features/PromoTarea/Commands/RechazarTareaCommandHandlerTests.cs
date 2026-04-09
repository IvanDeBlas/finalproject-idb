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
using PromoProgramaEntity = WePlayRises.Crowdpromotion.Domain.Model.PromoPrograma;
using Xunit;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.PromoTarea.Commands;

public class RechazarTareaCommandHandlerTests
{
    private readonly Mock<IPromoTareaService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<RechazarTareaCommand>> _validatorMock;
    private readonly Mock<ILogger<RechazarTareaCommandHandler>> _loggerMock;
    private readonly RechazarTareaCommandHandler _sut;

    public RechazarTareaCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromoTareaService>();
        _validatorMock = new Mock<IValidator<RechazarTareaCommand>>();
        _loggerMock = new Mock<ILogger<RechazarTareaCommandHandler>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PromoTareaProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _sut = new RechazarTareaCommandHandler(
            _serviceMock.Object,
            _mapper,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<RechazarTareaCommand>(), It.IsAny<CancellationToken>()))
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

        _serviceMock
            .Setup(s => s.RechazarTareaAsync(
                It.IsAny<PromoTareaPromotor>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        SetupHappyPath();
        var command = PromoTareaTestData.CreateValidRechazarCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EstadoTareaNombre.Should().Be("Rechazada");

        _serviceMock.Verify(
            s => s.RechazarTareaAsync(
                It.IsAny<PromoTareaPromotor>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = PromoTareaTestData.CreateValidRechazarCommand();
        var failures = new List<ValidationFailure>
        {
            new("ComentarioValidacion", "Required") { ErrorCode = ServiceResponseMessageType.Validation_Required }
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
        var command = PromoTareaTestData.CreateValidRechazarCommand();
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
        var command = PromoTareaTestData.CreateValidRechazarCommand();
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
    public async Task Handle_EstadoNoCompletada_ReturnsBadRequest()
    {
        // Arrange
        SetupHappyPath();
        var command = PromoTareaTestData.CreateValidRechazarCommand();
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
        var command = PromoTareaTestData.CreateValidRechazarCommand();
        _serviceMock
            .Setup(s => s.RechazarTareaAsync(
                It.IsAny<PromoTareaPromotor>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
