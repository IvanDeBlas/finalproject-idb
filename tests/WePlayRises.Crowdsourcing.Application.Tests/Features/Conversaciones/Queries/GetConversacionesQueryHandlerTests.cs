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

public class GetConversacionesQueryHandlerTests
{
    private readonly Mock<IConversacionCrowdsourcingService> _conversacionServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IPerfilProfesionalService> _perfilServiceMock;
    private readonly Mock<IValidator<GetConversacionesQuery>> _validatorMock;
    private readonly Mock<ILogger<GetConversacionesQueryHandler>> _loggerMock;
    private readonly GetConversacionesQueryHandler _sut;

    private readonly string _userId = Guid.NewGuid().ToString();

    public GetConversacionesQueryHandlerTests()
    {
        _conversacionServiceMock = new Mock<IConversacionCrowdsourcingService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _perfilServiceMock = new Mock<IPerfilProfesionalService>();
        _validatorMock = new Mock<IValidator<GetConversacionesQuery>>();
        _loggerMock = new Mock<ILogger<GetConversacionesQueryHandler>>();
        _sut = new GetConversacionesQueryHandler(
            _conversacionServiceMock.Object,
            _artistaServiceMock.Object,
            _perfilServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetConversacionesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsConversacionesList()
    {
        // Arrange
        var query = new GetConversacionesQuery { UserId = _userId, Contexto = "todas", Page = 1, PageSize = 20 };
        SetupValidValidation();

        var otherUserId = Guid.NewGuid().ToString();
        var conversaciones = new List<ConversacionCrowdsourcing>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserIdCreador = _userId,
                UserIdDestinatario = otherUserId,
                Asunto = "Test conv",
                FechaCreacion = DateTime.UtcNow,
                FechaUltimoMensaje = DateTime.UtcNow,
                Mensajes = new List<MensajeCrowdsourcing>
                {
                    new() { UserIdRemitente = otherUserId, Contenido = "Hola", Leido = false, FechaCreacion = DateTime.UtcNow }
                }
            }
        };

        _conversacionServiceMock
            .Setup(s => s.GetConversacionesByUserIdAsync(_userId, "todas", 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((conversaciones as IReadOnlyList<ConversacionCrowdsourcing>, 1, 1));

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(otherUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Artista { NombreArtistico = "Artista Test", ImagenPerfilUrl = "https://img.com/foto.jpg" });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.TotalCount.Should().Be(1);
        result.Data.TotalNoLeidos.Should().Be(1);
        result.Data.Items[0].NombreOtraParte.Should().Be("Artista Test");
        result.Data.Items[0].MensajesNoLeidos.Should().Be(1);
    }

    [Fact]
    public async Task Handle_EmptyList_ReturnsEmptyResponse()
    {
        // Arrange
        var query = new GetConversacionesQuery { UserId = _userId, Contexto = "todas", Page = 1, PageSize = 20 };
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.GetConversacionesByUserIdAsync(_userId, "todas", 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<ConversacionCrowdsourcing>() as IReadOnlyList<ConversacionCrowdsourcing>, 0, 0));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetConversacionesQuery { UserId = _userId };
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.GetConversacionesByUserIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
