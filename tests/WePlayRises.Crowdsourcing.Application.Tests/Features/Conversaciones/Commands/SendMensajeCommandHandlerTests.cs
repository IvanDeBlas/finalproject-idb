using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Conversaciones.Commands;

public class SendMensajeCommandHandlerTests
{
    private readonly Mock<IConversacionCrowdsourcingService> _conversacionServiceMock;
    private readonly Mock<IMensajeCrowdsourcingService> _mensajeServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IPerfilProfesionalService> _perfilServiceMock;
    private readonly Mock<IValidator<SendMensajeCommand>> _validatorMock;
    private readonly Mock<ILogger<SendMensajeCommandHandler>> _loggerMock;
    private readonly SendMensajeCommandHandler _sut;

    private readonly string _userId = Guid.NewGuid().ToString();
    private readonly Guid _conversacionId = Guid.NewGuid();

    public SendMensajeCommandHandlerTests()
    {
        _conversacionServiceMock = new Mock<IConversacionCrowdsourcingService>();
        _mensajeServiceMock = new Mock<IMensajeCrowdsourcingService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _perfilServiceMock = new Mock<IPerfilProfesionalService>();
        _validatorMock = new Mock<IValidator<SendMensajeCommand>>();
        _loggerMock = new Mock<ILogger<SendMensajeCommandHandler>>();
        _sut = new SendMensajeCommandHandler(
            _conversacionServiceMock.Object,
            _mensajeServiceMock.Object,
            _artistaServiceMock.Object,
            _perfilServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<SendMensajeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private SendMensajeCommand CreateValidCommand()
    {
        return new SendMensajeCommand
        {
            ConversacionId = _conversacionId,
            UserIdRemitente = _userId,
            Contenido = "Hola, mensaje de prueba",
            UrlAdjunto = null
        };
    }

    private ConversacionCrowdsourcing CreateConversacion()
    {
        return new ConversacionCrowdsourcing
        {
            Id = _conversacionId,
            UserIdCreador = _userId,
            UserIdDestinatario = Guid.NewGuid().ToString(),
            Asunto = "Test"
        };
    }

    [Fact]
    public async Task Handle_ValidCommand_SendsMensajeSuccessfully()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(_conversacionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateConversacion());

        _mensajeServiceMock
            .Setup(s => s.SendAsync(It.IsAny<MensajeCrowdsourcing>(), _conversacionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MensajeCrowdsourcing e, Guid _, CancellationToken _) => e);

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Artista { NombreArtistico = "Artista Test" });

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.Contenido.Should().Be("Hola, mensaje de prueba");
        result.Data.EsPropio.Should().BeTrue();
        result.Data.Leido.Should().BeFalse();
        result.Data.RemitenteNombre.Should().Be("Artista Test");
        result.Messages.Should().ContainSingle(m => m.HttpStatusCode == System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new("Contenido", "El contenido es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<SendMensajeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var command = new SendMensajeCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Handle_ConversacionNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(_conversacionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConversacionCrowdsourcing?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Conversacion);
    }

    [Fact]
    public async Task Handle_NotParticipant_ReturnsForbiddenError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UserIdRemitente = "not-a-participant";
        SetupValidValidation();

        var conv = new ConversacionCrowdsourcing
        {
            Id = _conversacionId,
            UserIdCreador = "user-a",
            UserIdDestinatario = "user-b",
            Asunto = "Test"
        };

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(_conversacionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conv);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(_conversacionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateConversacion());

        _mensajeServiceMock
            .Setup(s => s.SendAsync(It.IsAny<MensajeCrowdsourcing>(), _conversacionId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
