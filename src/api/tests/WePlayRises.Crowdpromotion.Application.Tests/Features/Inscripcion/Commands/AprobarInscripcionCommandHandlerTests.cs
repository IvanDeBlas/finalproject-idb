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

public class AprobarInscripcionCommandHandlerTests
{
    private readonly Mock<IInscripcionService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<AprobarInscripcionCommand>> _validatorMock;
    private readonly Mock<ILogger<AprobarInscripcionCommandHandler>> _loggerMock;
    private readonly AprobarInscripcionCommandHandler _sut;

    public AprobarInscripcionCommandHandlerTests()
    {
        _serviceMock = new Mock<IInscripcionService>();
        _validatorMock = new Mock<IValidator<AprobarInscripcionCommand>>();
        _loggerMock = new Mock<ILogger<AprobarInscripcionCommandHandler>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InscripcionProfile>());
        _mapper = config.CreateMapper();

        _sut = new AprobarInscripcionCommandHandler(
            _serviceMock.Object,
            _mapper,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsApprovedResult()
    {
        // Arrange
        var inscripcionId = Guid.NewGuid();
        var command = InscripcionTestData.CreateAprobarCommand(inscripcionId: inscripcionId);
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

        _serviceMock.Setup(s => s.CodigoReferidoExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _serviceMock.Setup(s => s.AprobarAsync(It.IsAny<PromoProgramaPromotor>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.EsAprobado.Should().BeTrue();
        result.Data.CodigoReferido.Should().NotBeNullOrEmpty();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Updated);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = InscripcionTestData.CreateAprobarCommand();

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
    public async Task Handle_NotOwner_ReturnsForbidden()
    {
        // Arrange
        var command = InscripcionTestData.CreateAprobarCommand();
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
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);
    }

    [Fact]
    public async Task Handle_InscripcionNotPending_ReturnsBadRequest()
    {
        // Arrange
        var inscripcionId = Guid.NewGuid();
        var command = InscripcionTestData.CreateAprobarCommand(inscripcionId: inscripcionId);
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
        var command = InscripcionTestData.CreateAprobarCommand();

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
