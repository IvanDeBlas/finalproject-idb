using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Valoraciones.Commands;

public class CreateValoracionCommandHandlerTests
{
    private readonly Mock<IValoracionCrowdsourcingService> _valoracionServiceMock;
    private readonly Mock<IAcuerdoCrowdsourcingService> _acuerdoServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IValidator<CreateValoracionCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateValoracionCommandHandler>> _loggerMock;
    private readonly CreateValoracionCommandHandler _sut;

    private readonly string _artistaUserId = Guid.NewGuid().ToString();
    private readonly string _proveedorUserId = Guid.NewGuid().ToString();
    private readonly ArtistaId _artistaId = new(Guid.NewGuid());
    private readonly Guid _acuerdoGuid = Guid.NewGuid();

    public CreateValoracionCommandHandlerTests()
    {
        _valoracionServiceMock = new Mock<IValoracionCrowdsourcingService>();
        _acuerdoServiceMock = new Mock<IAcuerdoCrowdsourcingService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _validatorMock = new Mock<IValidator<CreateValoracionCommand>>();
        _loggerMock = new Mock<ILogger<CreateValoracionCommandHandler>>();

        _sut = new CreateValoracionCommandHandler(
            _valoracionServiceMock.Object,
            _acuerdoServiceMock.Object,
            _artistaServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateValoracionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private AcuerdoCrowdsourcing CreateCompletedAcuerdo()
    {
        return new AcuerdoCrowdsourcing
        {
            Id = new AcuerdoCrowdsourcingId(_acuerdoGuid),
            ArtistaId = _artistaId,
            UserIdProveedor = _proveedorUserId,
            EstadoAcuerdoId = EstadoAcuerdoConstants.Completado,
            TituloInterno = "Test Acuerdo"
        };
    }

    private Artista CreateArtista()
    {
        return new Artista
        {
            Id = _artistaId,
            UserIdPropietario = _artistaUserId,
            NombreArtistico = "Test Artista"
        };
    }

    private CreateValoracionCommand CreateValidCommandAsArtista()
    {
        return new CreateValoracionCommand
        {
            AcuerdoId = _acuerdoGuid,
            UserId = _artistaUserId,
            Puntuacion = 5,
            Comentario = "Excelente trabajo"
        };
    }

    private CreateValoracionCommand CreateValidCommandAsProveedor()
    {
        return new CreateValoracionCommand
        {
            AcuerdoId = _acuerdoGuid,
            UserId = _proveedorUserId,
            Puntuacion = 4,
            Comentario = "Buen proyecto"
        };
    }

    [Fact]
    public async Task Handle_AsArtista_CreatesValoracionSuccessfully()
    {
        // Arrange
        var command = CreateValidCommandAsArtista();
        SetupValidValidation();

        _acuerdoServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCompletedAcuerdo());

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(_artistaUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateArtista());

        _valoracionServiceMock
            .Setup(s => s.ExisteValoracionAsync(_acuerdoGuid, _artistaUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _valoracionServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<ValoracionCrowdsourcing>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.HasErrors.Should().BeFalse();
        result.Data.Should().NotBeNull();
        result.Data!.Puntuacion.Should().Be(5);
        result.Data.Comentario.Should().Be("Excelente trabajo");

        _valoracionServiceMock.Verify(
            s => s.CreateAsync(
                It.Is<ValoracionCrowdsourcing>(v =>
                    v.UserIdAutor == _artistaUserId &&
                    v.UserIdValorado == _proveedorUserId &&
                    v.Puntuacion == 5),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_AsProveedor_SetsUserIdValoradoToArtista()
    {
        // Arrange
        var command = CreateValidCommandAsProveedor();
        SetupValidValidation();

        _acuerdoServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCompletedAcuerdo());

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(_proveedorUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        _artistaServiceMock
            .Setup(s => s.GetByIdAsync(_artistaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateArtista());

        _valoracionServiceMock
            .Setup(s => s.ExisteValoracionAsync(_acuerdoGuid, _proveedorUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _valoracionServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<ValoracionCrowdsourcing>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.HasErrors.Should().BeFalse();
        _valoracionServiceMock.Verify(
            s => s.CreateAsync(
                It.Is<ValoracionCrowdsourcing>(v =>
                    v.UserIdAutor == _proveedorUserId &&
                    v.UserIdValorado == _artistaUserId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = CreateValidCommandAsArtista();
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateValoracionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Puntuacion", "La puntuacion es obligatoria")
                {
                    ErrorCode = ServiceResponseMessageType.Validation_Required
                }
            }));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.HasErrors.Should().BeTrue();
        _valoracionServiceMock.Verify(
            s => s.CreateAsync(It.IsAny<ValoracionCrowdsourcing>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_AcuerdoNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = CreateValidCommandAsArtista();
        SetupValidValidation();

        _acuerdoServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcuerdoCrowdsourcing?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Acuerdo);
    }

    [Fact]
    public async Task Handle_UserNotParticipant_ReturnsForbidden()
    {
        // Arrange
        var command = CreateValidCommandAsArtista();
        command.UserId = Guid.NewGuid().ToString(); // random user, not participant
        SetupValidValidation();

        _acuerdoServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCompletedAcuerdo());

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_AcuerdoNotCompleted_ReturnsInvalidState()
    {
        // Arrange
        var command = CreateValidCommandAsArtista();
        SetupValidValidation();

        var acuerdo = CreateCompletedAcuerdo();
        acuerdo.EstadoAcuerdoId = EstadoAcuerdoConstants.Activo;

        _acuerdoServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(_artistaUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateArtista());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_InvalidState);
    }

    [Fact]
    public async Task Handle_AlreadyRated_ReturnsDuplicateAction()
    {
        // Arrange
        var command = CreateValidCommandAsArtista();
        SetupValidValidation();

        _acuerdoServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCompletedAcuerdo());

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(_artistaUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateArtista());

        _valoracionServiceMock
            .Setup(s => s.ExisteValoracionAsync(_acuerdoGuid, _artistaUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_DuplicateAction);
    }

    [Fact]
    public async Task Handle_UnexpectedException_ReturnsInternalError()
    {
        // Arrange
        var command = CreateValidCommandAsArtista();
        SetupValidValidation();

        _acuerdoServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
