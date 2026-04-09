using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Features.Artistas.Commands;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Application.Tests.Helpers;
using WePlayRises.UserAccess.Domain.Constants;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Application.Tests.Features.Artistas.Commands;

public class CreateArtistaCommandHandlerTests
{
    private readonly Mock<IArtistaService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateArtistaCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateArtistaCommandHandler>> _loggerMock;
    private readonly CreateArtistaCommandHandler _sut;

    public CreateArtistaCommandHandlerTests()
    {
        _serviceMock = new Mock<IArtistaService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreateArtistaCommand>>();
        _loggerMock = new Mock<ILogger<CreateArtistaCommandHandler>>();
        _sut = new CreateArtistaCommandHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithArtistaDto()
    {
        // Arrange
        var userId = "user-123";
        var artistaId = Guid.NewGuid();
        var command = new CreateArtistaCommand
        {
            NombreArtistico = "Test Artist",
            Descripcion = "Test description",
            Pais = "Spain",
            Ciudad = "Madrid",
            UserId = userId,
        };

        var entity = ArtistaTestData.CreateValid(artistaId, userId);
        var expectedDto = ArtistaTestData.CreateValidDto(artistaId, userId);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        _mapperMock
            .Setup(m => m.Map<Artista>(command))
            .Returns(entity);

        var returnedId = new ArtistaId(artistaId);
        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Artista>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnedId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<ArtistaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(m => m.Map<ArtistaDto>(entity))
            .Returns(expectedDto);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.NombreArtistico.Should().Be("Test Artist");
        _serviceMock.Verify(s => s.CreateAsync(It.IsAny<Artista>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = new CreateArtistaCommand
        {
            NombreArtistico = "",
            UserId = "",
        };

        var validationFailures = new List<ValidationFailure>
        {
            new("NombreArtistico", "El nombre artistico es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
            new("UserId", "El UserId es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().HaveCount(2);
        _serviceMock.Verify(s => s.CreateAsync(It.IsAny<Artista>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_EmptyUserId_ReturnsUnauthorized()
    {
        // Arrange
        var command = new CreateArtistaCommand
        {
            NombreArtistico = "Test Artist",
            UserId = null,
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Unauthorized);
    }

    [Fact]
    public async Task Handle_UserAlreadyHasArtista_ReturnsConflict()
    {
        // Arrange
        var userId = "user-123";
        var command = new CreateArtistaCommand
        {
            NombreArtistico = "Test Artist",
            UserId = userId,
        };

        var existingArtista = ArtistaTestData.CreateValid(userId: userId);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingArtista);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_ArtistaAlreadyExists);
        _serviceMock.Verify(s => s.CreateAsync(It.IsAny<Artista>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var userId = "user-123";
        var command = new CreateArtistaCommand
        {
            NombreArtistico = "Test Artist",
            UserId = userId,
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        _mapperMock
            .Setup(m => m.Map<Artista>(command))
            .Returns(ArtistaTestData.CreateValid());

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Artista>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
