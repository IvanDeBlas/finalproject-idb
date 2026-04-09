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

public class UpdateArtistaCommandHandlerTests
{
    private readonly Mock<IArtistaService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<UpdateArtistaCommand>> _validatorMock;
    private readonly Mock<ILogger<UpdateArtistaCommandHandler>> _loggerMock;
    private readonly UpdateArtistaCommandHandler _sut;

    public UpdateArtistaCommandHandlerTests()
    {
        _serviceMock = new Mock<IArtistaService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<UpdateArtistaCommand>>();
        _loggerMock = new Mock<ILogger<UpdateArtistaCommandHandler>>();
        _sut = new UpdateArtistaCommandHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithUpdatedDto()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var userId = "user-123";
        var command = new UpdateArtistaCommand
        {
            Id = artistaId,
            NombreArtistico = "Updated Artist",
            Descripcion = "Updated description",
            Pais = "Spain",
            Ciudad = "Barcelona",
            UserId = userId,
        };

        var existingEntity = ArtistaTestData.CreateValid(artistaId, userId);
        var expectedDto = ArtistaTestData.CreateValidDto(artistaId, userId, "Updated Artist");

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEntity);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Artista>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<ArtistaDto>(It.IsAny<Artista>()))
            .Returns(expectedDto);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.NombreArtistico.Should().Be("Updated Artist");
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<Artista>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = new UpdateArtistaCommand
        {
            Id = Guid.Empty,
            NombreArtistico = "",
            UserId = "",
        };

        var validationFailures = new List<ValidationFailure>
        {
            new("Id", "El Id del artista es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
            new("NombreArtistico", "El nombre artistico es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
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
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<Artista>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var command = new UpdateArtistaCommand
        {
            Id = artistaId,
            NombreArtistico = "Updated Artist",
            UserId = "user-123",
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Artista);
    }

    [Fact]
    public async Task Handle_NotOwner_ReturnsForbidden()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var command = new UpdateArtistaCommand
        {
            Id = artistaId,
            NombreArtistico = "Updated Artist",
            UserId = "different-user",
        };

        var existingEntity = ArtistaTestData.CreateValid(artistaId, "original-owner");

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEntity);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<Artista>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var userId = "user-123";
        var command = new UpdateArtistaCommand
        {
            Id = artistaId,
            NombreArtistico = "Updated Artist",
            UserId = userId,
        };

        var existingEntity = ArtistaTestData.CreateValid(artistaId, userId);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new ArtistaId(artistaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEntity);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Artista>(), It.IsAny<CancellationToken>()))
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
