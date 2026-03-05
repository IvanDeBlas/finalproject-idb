using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Campanias.Commands;

public class PublishCampaniaCommandHandlerTests
{
    private readonly Mock<ICampaniaService> _serviceMock;
    private readonly Mock<IValidator<PublishCampaniaCommand>> _validatorMock;
    private readonly Mock<ILogger<PublishCampaniaCommandHandler>> _loggerMock;
    private readonly PublishCampaniaCommandHandler _sut;

    public PublishCampaniaCommandHandlerTests()
    {
        _serviceMock = new Mock<ICampaniaService>();
        _validatorMock = new Mock<IValidator<PublishCampaniaCommand>>();
        _loggerMock = new Mock<ILogger<PublishCampaniaCommandHandler>>();
        _sut = new PublishCampaniaCommandHandler(
            _serviceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidPublish_ReturnsSuccessWithResponse()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var campaniaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreateValidBorrador(artistaId, campaniaId);

        var command = new PublishCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = artistaId
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(campaniaId);
        result.Data.EstadoCampaniaId.Should().Be(2);
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsErrors()
    {
        // Arrange
        var command = new PublishCampaniaCommand
        {
            Id = Guid.Empty,
            ArtistaId = Guid.Empty
        };

        var validationFailures = new List<ValidationFailure>
        {
            new("Id", "El Id es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
            new("ArtistaId", "El ArtistaId es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().HaveCount(2);
        _serviceMock.Verify(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CampaniaNotFound_ReturnsNotFound()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var command = new PublishCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = Guid.NewGuid()
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CampaniaCrowdfunding?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Campania);
    }

    [Fact]
    public async Task Handle_NotOwner_ReturnsForbidden()
    {
        // Arrange
        var ownerArtistaId = Guid.NewGuid();
        var attackerArtistaId = Guid.NewGuid();
        var campaniaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreateValidBorrador(ownerArtistaId, campaniaId);

        var command = new PublishCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = attackerArtistaId
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NotDraft_ReturnsBusinessError()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var campaniaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreatePublicada(artistaId, campaniaId);

        var command = new PublishCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = artistaId
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_CampaniaNotDraft);
    }

    [Fact]
    public async Task Handle_MissingTitulo_ReturnsValidationError()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var campaniaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreateValidBorrador(artistaId, campaniaId);
        entity.Titulo = string.Empty;

        var command = new PublishCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = artistaId
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m =>
            m.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Handle_InvalidImporteObjetivo_ReturnsValidationError()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var campaniaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreateValidBorrador(artistaId, campaniaId);
        entity.ImporteObjetivo = 0;

        var command = new PublishCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = artistaId
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m =>
            m.ErrorCode == ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Handle_FechaFinTooSoon_ReturnsValidationError()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var campaniaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreateValidBorrador(artistaId, campaniaId);
        entity.FechaFin = DateTime.UtcNow.AddDays(3); // Less than 7 days

        var command = new PublishCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = artistaId
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m =>
            m.ErrorCode == ServiceResponseMessageType.Validation_InvalidDate);
    }

    [Fact]
    public async Task Handle_MultipleMissingFields_ReturnsAllErrors()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var campaniaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreateValidBorrador(artistaId, campaniaId);
        entity.Titulo = string.Empty;
        entity.ImporteObjetivo = 0;
        entity.MonedaId = 0;
        entity.TipoFinanciacionId = 0;
        entity.FechaFin = DateTime.UtcNow.AddDays(2);

        var command = new PublishCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = artistaId
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().HaveCountGreaterThanOrEqualTo(5);
    }

    [Fact]
    public async Task Handle_SetsPublicationDate_WhenPublished()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var campaniaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreateValidBorrador(artistaId, campaniaId);
        entity.FechaPublicacion = null;

        var command = new PublishCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = artistaId
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        CampaniaCrowdfunding? capturedEntity = null;
        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()))
            .Callback<CampaniaCrowdfunding, CancellationToken>((e, _) => capturedEntity = e)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedEntity.Should().NotBeNull();
        capturedEntity!.FechaPublicacion.Should().NotBeNull();
        capturedEntity.FechaPublicacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Data.FechaPublicacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_SetsFechaInicio_WhenNull()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var campaniaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreateValidBorrador(artistaId, campaniaId);
        entity.FechaInicio = null;

        var command = new PublishCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = artistaId
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        CampaniaCrowdfunding? capturedEntity = null;
        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()))
            .Callback<CampaniaCrowdfunding, CancellationToken>((e, _) => capturedEntity = e)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedEntity.Should().NotBeNull();
        capturedEntity!.FechaInicio.Should().NotBeNull();
        capturedEntity.FechaInicio.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
