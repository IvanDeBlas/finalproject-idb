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

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Conversaciones.Commands;

public class MarcarLeidosCommandHandlerTests
{
    private readonly Mock<IConversacionCrowdsourcingService> _conversacionServiceMock;
    private readonly Mock<IMensajeCrowdsourcingService> _mensajeServiceMock;
    private readonly Mock<IValidator<MarcarLeidosCommand>> _validatorMock;
    private readonly Mock<ILogger<MarcarLeidosCommandHandler>> _loggerMock;
    private readonly MarcarLeidosCommandHandler _sut;

    private readonly string _userId = Guid.NewGuid().ToString();
    private readonly Guid _conversacionId = Guid.NewGuid();

    public MarcarLeidosCommandHandlerTests()
    {
        _conversacionServiceMock = new Mock<IConversacionCrowdsourcingService>();
        _mensajeServiceMock = new Mock<IMensajeCrowdsourcingService>();
        _validatorMock = new Mock<IValidator<MarcarLeidosCommand>>();
        _loggerMock = new Mock<ILogger<MarcarLeidosCommandHandler>>();
        _sut = new MarcarLeidosCommandHandler(
            _conversacionServiceMock.Object,
            _mensajeServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<MarcarLeidosCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidCommand_MarksMessagesAsRead()
    {
        // Arrange
        var command = new MarcarLeidosCommand { ConversacionId = _conversacionId, UserId = _userId };
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(_conversacionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConversacionCrowdsourcing
            {
                Id = _conversacionId,
                UserIdCreador = _userId,
                UserIdDestinatario = "other-user"
            });

        _mensajeServiceMock
            .Setup(s => s.MarcarLeidosAsync(_conversacionId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.MensajesMarcados.Should().Be(3);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new("ConversacionId", "El ID es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<MarcarLeidosCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var command = new MarcarLeidosCommand();

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
        var command = new MarcarLeidosCommand { ConversacionId = _conversacionId, UserId = _userId };
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
        var command = new MarcarLeidosCommand { ConversacionId = _conversacionId, UserId = "not-participant" };
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(_conversacionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConversacionCrowdsourcing
            {
                Id = _conversacionId,
                UserIdCreador = "user-a",
                UserIdDestinatario = "user-b"
            });

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
        var command = new MarcarLeidosCommand { ConversacionId = _conversacionId, UserId = _userId };
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(_conversacionId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_ZeroUnreadMessages_ReturnsSuccessWithZero()
    {
        // Arrange
        var command = new MarcarLeidosCommand { ConversacionId = _conversacionId, UserId = _userId };
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(_conversacionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConversacionCrowdsourcing
            {
                Id = _conversacionId,
                UserIdCreador = _userId,
                UserIdDestinatario = "other-user"
            });

        _mensajeServiceMock
            .Setup(s => s.MarcarLeidosAsync(_conversacionId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.MensajesMarcados.Should().Be(0);
    }
}
