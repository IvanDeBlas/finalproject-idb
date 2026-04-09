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

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Inscripcion.Commands;

public class RechazarInscripcionCommandHandlerTests
{
    private readonly Mock<IInscripcionService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<RechazarInscripcionCommand>> _validatorMock;
    private readonly Mock<ILogger<RechazarInscripcionCommandHandler>> _loggerMock;
    private readonly RechazarInscripcionCommandHandler _sut;

    public RechazarInscripcionCommandHandlerTests()
    {
        _serviceMock = new Mock<IInscripcionService>();
        _validatorMock = new Mock<IValidator<RechazarInscripcionCommand>>();
        _loggerMock = new Mock<ILogger<RechazarInscripcionCommandHandler>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InscripcionProfile>());
        _mapper = config.CreateMapper();

        _sut = new RechazarInscripcionCommandHandler(
            _serviceMock.Object,
            _mapper,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsRejectedResult()
    {
        // Arrange
        var inscripcionId = Guid.NewGuid();
        var command = InscripcionTestData.CreateRechazarCommand(inscripcionId: inscripcionId);
        var programa = InscripcionTestData.CreateValidPrograma(artistaId: InscripcionTestData.DefaultArtistaId);
        var inscripcion = InscripcionTestData.CreatePendienteInscripcion(id: inscripcionId);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(InscripcionTestData.DefaultArtistaId);

        _serviceMock.Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock.Setup(s => s.GetByIdWithPromotorAsync(inscripcionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inscripcion);

        _serviceMock.Setup(s => s.RechazarAsync(inscripcionId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.InscripcionId.Should().Be(inscripcionId);
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Deleted);
    }

    [Fact]
    public async Task Handle_InscripcionAlreadyApproved_ReturnsBadRequest()
    {
        // Arrange
        var inscripcionId = Guid.NewGuid();
        var command = InscripcionTestData.CreateRechazarCommand(inscripcionId: inscripcionId);
        var programa = InscripcionTestData.CreateValidPrograma(artistaId: InscripcionTestData.DefaultArtistaId);
        var approved = InscripcionTestData.CreateAprobadaInscripcion(id: inscripcionId);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(InscripcionTestData.DefaultArtistaId);

        _serviceMock.Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock.Setup(s => s.GetByIdWithPromotorAsync(inscripcionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(approved);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_InscripcionEstadoInvalido);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalServerError()
    {
        // Arrange
        var command = InscripcionTestData.CreateRechazarCommand();

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

public class BloquearInscripcionCommandHandlerTests
{
    private readonly Mock<IInscripcionService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<BloquearInscripcionCommand>> _validatorMock;
    private readonly Mock<ILogger<BloquearInscripcionCommandHandler>> _loggerMock;
    private readonly BloquearInscripcionCommandHandler _sut;

    public BloquearInscripcionCommandHandlerTests()
    {
        _serviceMock = new Mock<IInscripcionService>();
        _validatorMock = new Mock<IValidator<BloquearInscripcionCommand>>();
        _loggerMock = new Mock<ILogger<BloquearInscripcionCommandHandler>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InscripcionProfile>());
        _mapper = config.CreateMapper();

        _sut = new BloquearInscripcionCommandHandler(
            _serviceMock.Object,
            _mapper,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidPendingInscripcion_ReturnsBlockedResult()
    {
        // Arrange
        var inscripcionId = Guid.NewGuid();
        var command = InscripcionTestData.CreateBloquearCommand(inscripcionId: inscripcionId);
        var programa = InscripcionTestData.CreateValidPrograma(artistaId: InscripcionTestData.DefaultArtistaId);
        var inscripcion = InscripcionTestData.CreatePendienteInscripcion(id: inscripcionId);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(InscripcionTestData.DefaultArtistaId);

        _serviceMock.Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock.Setup(s => s.GetByIdWithPromotorAsync(inscripcionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inscripcion);

        _serviceMock.Setup(s => s.BloquearAsync(It.IsAny<PromoProgramaPromotor>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.EsBloqueado.Should().BeTrue();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Updated);
    }

    [Fact]
    public async Task Handle_AlreadyBlocked_ReturnsBadRequest()
    {
        // Arrange
        var inscripcionId = Guid.NewGuid();
        var command = InscripcionTestData.CreateBloquearCommand(inscripcionId: inscripcionId);
        var programa = InscripcionTestData.CreateValidPrograma(artistaId: InscripcionTestData.DefaultArtistaId);
        var blocked = InscripcionTestData.CreateBloqueadaInscripcion(id: inscripcionId);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(InscripcionTestData.DefaultArtistaId);

        _serviceMock.Setup(s => s.GetProgramaByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock.Setup(s => s.GetByIdWithPromotorAsync(inscripcionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(blocked);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_InscripcionEstadoInvalido);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalServerError()
    {
        // Arrange
        var command = InscripcionTestData.CreateBloquearCommand();

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
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Updated);
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
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_InscripcionEstadoInvalido);
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
