using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Campanias.Commands;

public class UpdateCampaniaCommandHandlerTests
{
    private readonly Mock<ICampaniaService> _serviceMock;
    private readonly Mock<IValidator<UpdateCampaniaCommand>> _validatorMock;
    private readonly Mock<ILogger<UpdateCampaniaCommandHandler>> _loggerMock;
    private readonly UpdateCampaniaCommandHandler _sut;

    public UpdateCampaniaCommandHandlerTests()
    {
        _serviceMock = new Mock<ICampaniaService>();
        _validatorMock = new Mock<IValidator<UpdateCampaniaCommand>>();
        _loggerMock = new Mock<ILogger<UpdateCampaniaCommandHandler>>();
        _sut = new UpdateCampaniaCommandHandler(
            _serviceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccess()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var campaniaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreateValidBorrador(artistaId, campaniaId);

        var command = new UpdateCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = artistaId,
            Titulo = "Updated Title"
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
        result.Data.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsErrors()
    {
        // Arrange
        var command = new UpdateCampaniaCommand
        {
            Id = Guid.Empty,
            ArtistaId = Guid.NewGuid()
        };

        var validationFailures = new List<ValidationFailure>
        {
            new("Id", "El Id es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        _serviceMock.Verify(s => s.GetByIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CampaniaNotFound_ReturnsNotFound()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var command = new UpdateCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = Guid.NewGuid(),
            Titulo = "Updated"
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

        var command = new UpdateCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = attackerArtistaId,
            Titulo = "Hacked Title"
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

        var command = new UpdateCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = artistaId,
            Titulo = "Try to update published"
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
    public async Task Handle_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var campaniaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreateValidBorrador(artistaId, campaniaId);
        var originalSubtitulo = entity.Subtitulo;
        var originalDescripcion = entity.DescripcionCorta;

        var command = new UpdateCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = artistaId,
            Titulo = "Only Title Changed"
            // All other fields are null (not provided)
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        CampaniaCrowdfunding? updatedEntity = null;
        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()))
            .Callback<CampaniaCrowdfunding, CancellationToken>((e, _) => updatedEntity = e)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        updatedEntity.Should().NotBeNull();
        updatedEntity!.Titulo.Should().Be("Only Title Changed");
        updatedEntity.Subtitulo.Should().Be(originalSubtitulo);
        updatedEntity.DescripcionCorta.Should().Be(originalDescripcion);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var campaniaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreateValidBorrador(artistaId, campaniaId);

        var command = new UpdateCampaniaCommand
        {
            Id = campaniaId,
            ArtistaId = artistaId,
            Titulo = "Will Fail"
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
