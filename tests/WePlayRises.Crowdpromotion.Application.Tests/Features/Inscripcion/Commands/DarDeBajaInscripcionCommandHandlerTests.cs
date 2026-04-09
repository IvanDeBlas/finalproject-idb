using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Mapping;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;
using PromoProgramaEntity = WePlayRises.Crowdpromotion.Domain.Model.PromoPrograma;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Inscripcion.Commands;

public class DarDeBajaInscripcionCommandHandlerTests
{
    private readonly Mock<IInscripcionService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<DarDeBajaInscripcionCommand>> _validatorMock;
    private readonly Mock<ILogger<DarDeBajaInscripcionCommandHandler>> _loggerMock;
    private readonly DarDeBajaInscripcionCommandHandler _sut;

    public DarDeBajaInscripcionCommandHandlerTests()
    {
        _serviceMock = new Mock<IInscripcionService>();
        _validatorMock = new Mock<IValidator<DarDeBajaInscripcionCommand>>();
        _loggerMock = new Mock<ILogger<DarDeBajaInscripcionCommandHandler>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InscripcionProfile>());
        _mapper = config.CreateMapper();

        _sut = new DarDeBajaInscripcionCommandHandler(
            _serviceMock.Object,
            _mapper,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidApprovedInscripcion_ReturnsDadaDeBajaResult()
    {
        // Arrange
        var inscripcionId = Guid.NewGuid();
        var command = InscripcionTestData.CreateDarDeBajaCommand(inscripcionId: inscripcionId);
        var programa = InscripcionTestData.CreateValidPrograma(artistaId: InscripcionTestData.DefaultArtistaId);
        var inscripcion = InscripcionTestData.CreateAprobadaInscripcion(id: inscripcionId);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(InscripcionTestData.DefaultArtistaId);

        _serviceMock.Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock.Setup(s => s.GetByIdWithPromotorAsync(inscripcionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inscripcion);

        _serviceMock.Setup(s => s.DarDeBajaAsync(It.IsAny<PromoProgramaPromotor>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.EsAprobado.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Updated);

        _serviceMock.Verify(
            s => s.DarDeBajaAsync(It.IsAny<PromoProgramaPromotor>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = InscripcionTestData.CreateDarDeBajaCommand();
        var failures = new List<ValidationFailure>
        {
            new("InscripcionId", "El InscripcionId es obligatorio")
            {
                ErrorCode = ServiceResponseMessageType.Validation_Required
            }
        };

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().NotBeEmpty();

        _serviceMock.Verify(
            s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = InscripcionTestData.CreateDarDeBajaCommand();

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArtistaId?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Artista);
    }

    [Fact]
    public async Task Handle_ProgramaNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = InscripcionTestData.CreateDarDeBajaCommand();

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(InscripcionTestData.DefaultArtistaId);

        _serviceMock.Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromoProgramaEntity?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.NotFound_PromoPrograma);
    }

    [Fact]
    public async Task Handle_NotOwner_ReturnsForbidden()
    {
        // Arrange
        var command = InscripcionTestData.CreateDarDeBajaCommand();
        var differentArtistaId = new ArtistaId(Guid.NewGuid());
        var programa = InscripcionTestData.CreateValidPrograma(artistaId: differentArtistaId);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(InscripcionTestData.DefaultArtistaId);

        _serviceMock.Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);
    }

    [Fact]
    public async Task Handle_InscripcionNotFound_ReturnsNotFound()
    {
        // Arrange
        var inscripcionId = Guid.NewGuid();
        var command = InscripcionTestData.CreateDarDeBajaCommand(inscripcionId: inscripcionId);
        var programa = InscripcionTestData.CreateValidPrograma(artistaId: InscripcionTestData.DefaultArtistaId);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(InscripcionTestData.DefaultArtistaId);

        _serviceMock.Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock.Setup(s => s.GetByIdWithPromotorAsync(inscripcionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromoProgramaPromotor?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Inscripcion);
    }

    [Fact]
    public async Task Handle_InscripcionNotApproved_ReturnsBadRequest()
    {
        // Arrange
        var inscripcionId = Guid.NewGuid();
        var command = InscripcionTestData.CreateDarDeBajaCommand(inscripcionId: inscripcionId);
        var programa = InscripcionTestData.CreateValidPrograma(artistaId: InscripcionTestData.DefaultArtistaId);
        var pending = InscripcionTestData.CreatePendienteInscripcion(id: inscripcionId);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(InscripcionTestData.DefaultArtistaId);

        _serviceMock.Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock.Setup(s => s.GetByIdWithPromotorAsync(inscripcionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pending);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_InscripcionEstadoInvalido);
    }

    [Fact]
    public async Task Handle_InscripcionAlreadyDadaDeBaja_ReturnsBadRequest()
    {
        // Arrange
        var inscripcionId = Guid.NewGuid();
        var command = InscripcionTestData.CreateDarDeBajaCommand(inscripcionId: inscripcionId);
        var programa = InscripcionTestData.CreateValidPrograma(artistaId: InscripcionTestData.DefaultArtistaId);
        var dadaDeBaja = InscripcionTestData.CreateDadaDeBajaInscripcion(id: inscripcionId);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(InscripcionTestData.DefaultArtistaId);

        _serviceMock.Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock.Setup(s => s.GetByIdWithPromotorAsync(inscripcionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dadaDeBaja);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_InscripcionEstadoInvalido);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalServerError()
    {
        // Arrange
        var command = InscripcionTestData.CreateDarDeBajaCommand();

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
