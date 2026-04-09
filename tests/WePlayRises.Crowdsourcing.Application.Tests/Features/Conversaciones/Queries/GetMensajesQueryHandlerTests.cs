using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Queries;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Conversaciones.Queries;

public class GetMensajesQueryHandlerTests
{
    private readonly Mock<IConversacionCrowdsourcingService> _conversacionServiceMock;
    private readonly Mock<IMensajeCrowdsourcingService> _mensajeServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IPerfilProfesionalService> _perfilServiceMock;
    private readonly Mock<IValidator<GetMensajesQuery>> _validatorMock;
    private readonly Mock<ILogger<GetMensajesQueryHandler>> _loggerMock;
    private readonly GetMensajesQueryHandler _sut;

    private readonly string _userId = Guid.NewGuid().ToString();
    private readonly Guid _conversacionId = Guid.NewGuid();

    public GetMensajesQueryHandlerTests()
    {
        _conversacionServiceMock = new Mock<IConversacionCrowdsourcingService>();
        _mensajeServiceMock = new Mock<IMensajeCrowdsourcingService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _perfilServiceMock = new Mock<IPerfilProfesionalService>();
        _validatorMock = new Mock<IValidator<GetMensajesQuery>>();
        _loggerMock = new Mock<ILogger<GetMensajesQueryHandler>>();
        _sut = new GetMensajesQueryHandler(
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
            .Setup(v => v.ValidateAsync(It.IsAny<GetMensajesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsMensajesList()
    {
        // Arrange
        var query = new GetMensajesQuery { ConversacionId = _conversacionId, UserId = _userId, Page = 1, PageSize = 50 };
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(_conversacionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConversacionCrowdsourcing { Id = _conversacionId, UserIdCreador = _userId, UserIdDestinatario = "other" });

        var mensajes = new List<MensajeCrowdsourcing>
        {
            new() { Id = Guid.NewGuid(), UserIdRemitente = _userId, Contenido = "Hola", Leido = false, FechaCreacion = DateTime.UtcNow }
        };
        _mensajeServiceMock
            .Setup(s => s.GetByConversacionIdPaginatedAsync(_conversacionId, 1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync((mensajes as IReadOnlyList<MensajeCrowdsourcing>, 1));

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Artista { NombreArtistico = "Remitente" });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items[0].EsPropio.Should().BeTrue();
        result.Data.Items[0].RemitenteNombre.Should().Be("Remitente");
    }

    [Fact]
    public async Task Handle_ConversacionNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var query = new GetMensajesQuery { ConversacionId = _conversacionId, UserId = _userId };
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(_conversacionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConversacionCrowdsourcing?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Conversacion);
    }

    [Fact]
    public async Task Handle_NotParticipant_ReturnsForbiddenError()
    {
        // Arrange
        var query = new GetMensajesQuery { ConversacionId = _conversacionId, UserId = "not-participant" };
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(_conversacionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConversacionCrowdsourcing { Id = _conversacionId, UserIdCreador = "user-a", UserIdDestinatario = "user-b" });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetMensajesQuery { ConversacionId = _conversacionId, UserId = _userId };
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(_conversacionId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
