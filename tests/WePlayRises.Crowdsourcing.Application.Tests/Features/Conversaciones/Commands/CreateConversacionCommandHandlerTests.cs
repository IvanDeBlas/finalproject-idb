using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Conversaciones.Commands;

public class CreateConversacionCommandHandlerTests
{
    private readonly Mock<IConversacionCrowdsourcingService> _conversacionServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IPerfilProfesionalService> _perfilServiceMock;
    private readonly Mock<IValidator<CreateConversacionCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateConversacionCommandHandler>> _loggerMock;
    private readonly CreateConversacionCommandHandler _sut;

    public CreateConversacionCommandHandlerTests()
    {
        _conversacionServiceMock = new Mock<IConversacionCrowdsourcingService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _perfilServiceMock = new Mock<IPerfilProfesionalService>();
        _validatorMock = new Mock<IValidator<CreateConversacionCommand>>();
        _loggerMock = new Mock<ILogger<CreateConversacionCommandHandler>>();
        _sut = new CreateConversacionCommandHandler(
            _conversacionServiceMock.Object,
            _artistaServiceMock.Object,
            _perfilServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateConversacionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private CreateConversacionCommand CreateValidCommand()
    {
        return new CreateConversacionCommand
        {
            NecesidadId = Guid.NewGuid(),
            AcuerdoId = null,
            UserIdDestinatario = Guid.NewGuid().ToString(),
            Asunto = "Consulta sobre la necesidad",
            UserIdCreador = Guid.NewGuid().ToString()
        };
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesConversacionSuccessfully()
    {
        // Arrange
        var command = CreateValidCommand();
        var createdId = Guid.NewGuid();
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.ExisteConversacionParaContextoAsync(
                command.UserIdCreador, command.UserIdDestinatario,
                command.NecesidadId, command.AcuerdoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _conversacionServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<ConversacionCrowdsourcing>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdId);

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(createdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConversacionCrowdsourcing
            {
                Id = createdId,
                UserIdCreador = command.UserIdCreador,
                UserIdDestinatario = command.UserIdDestinatario,
                Asunto = command.Asunto
            });

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(command.UserIdDestinatario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Artista { NombreArtistico = "Artista Destinatario" });

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(createdId);
        result.Data.Asunto.Should().Be("Consulta sobre la necesidad");
        result.Data.NombreDestinatario.Should().Be("Artista Destinatario");
        result.Messages.Should().ContainSingle(m => m.HttpStatusCode == System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new("Asunto", "El asunto es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateConversacionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var command = new CreateConversacionCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Validation_Required);
        _conversacionServiceMock.Verify(s =>
            s.CreateAsync(It.IsAny<ConversacionCrowdsourcing>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DuplicateConversacion_ReturnsDuplicateError()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.ExisteConversacionParaContextoAsync(
                command.UserIdCreador, command.UserIdDestinatario,
                command.NecesidadId, command.AcuerdoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_ConversacionDuplicada);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.ExisteConversacionParaContextoAsync(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _conversacionServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<ConversacionCrowdsourcing>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_DestinatarioIsPerfilProfesional_UseTituloAsFallback()
    {
        // Arrange
        var command = CreateValidCommand();
        var createdId = Guid.NewGuid();
        SetupValidValidation();

        _conversacionServiceMock
            .Setup(s => s.ExisteConversacionParaContextoAsync(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _conversacionServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<ConversacionCrowdsourcing>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdId);

        _conversacionServiceMock
            .Setup(s => s.GetByIdAsync(createdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConversacionCrowdsourcing { Id = createdId, Asunto = command.Asunto });

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(command.UserIdDestinatario, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        _perfilServiceMock
            .Setup(s => s.GetByUserIdAsync(command.UserIdDestinatario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PerfilProfesional { Titulo = "Ingeniero de Sonido" });

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.NombreDestinatario.Should().Be("Ingeniero de Sonido");
    }
}
