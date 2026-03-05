using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Commands;

public class UpdateMilestoneCommandHandlerTests
{
    private readonly Mock<IAcuerdoCrowdsourcingService> _acuerdoServiceMock;
    private readonly Mock<IAcuerdoCrowdsourcingMilestoneService> _milestoneServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IValidator<UpdateMilestoneCommand>> _validatorMock;
    private readonly Mock<ILogger<UpdateMilestoneCommandHandler>> _loggerMock;
    private readonly UpdateMilestoneCommandHandler _sut;

    public UpdateMilestoneCommandHandlerTests()
    {
        _acuerdoServiceMock = new Mock<IAcuerdoCrowdsourcingService>();
        _milestoneServiceMock = new Mock<IAcuerdoCrowdsourcingMilestoneService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _validatorMock = new Mock<IValidator<UpdateMilestoneCommand>>();
        _loggerMock = new Mock<ILogger<UpdateMilestoneCommandHandler>>();
        _sut = new UpdateMilestoneCommandHandler(
            _acuerdoServiceMock.Object,
            _milestoneServiceMock.Object,
            _artistaServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<UpdateMilestoneCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesMilestone()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(artistaId, "user-artista");
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(artistaId);
        var milestone = CrowdsourcingTestData.CreateMilestone(acuerdo.Id);

        var command = new UpdateMilestoneCommand
        {
            MilestoneId = milestone.Id, AcuerdoId = acuerdo.Id.Value, UserId = "user-artista",
            Titulo = "Fase 1 Actualizada", ImporteParcial = 400m
        };

        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-artista", It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);
        _milestoneServiceMock.Setup(s => s.GetByIdAsync(milestone.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(milestone);
        _milestoneServiceMock.Setup(s => s.GetSumaImportesAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(400m);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.Titulo.Should().Be("Fase 1 Actualizada");
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MilestoneCompleted_ReturnsConflict()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(artistaId, "user-artista");
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(artistaId);
        var milestone = CrowdsourcingTestData.CreateMilestone(acuerdo.Id, completado: true);

        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-artista", It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);
        _milestoneServiceMock.Setup(s => s.GetByIdAsync(milestone.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(milestone);

        // Act
        var result = await _sut.Handle(new UpdateMilestoneCommand
        {
            MilestoneId = milestone.Id, AcuerdoId = acuerdo.Id.Value, UserId = "user-artista",
            Titulo = "Update", ImporteParcial = 100
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_MilestoneCompleted);
    }

    [Fact]
    public async Task Handle_NotArtista_ReturnsForbidden()
    {
        // Arrange
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo();

        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("other-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAccess.Domain.Model.Artista?)null);

        // Act
        var result = await _sut.Handle(new UpdateMilestoneCommand
        {
            MilestoneId = Guid.NewGuid(), AcuerdoId = acuerdo.Id.Value, UserId = "other-user",
            Titulo = "Update", ImporteParcial = 100
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(new UpdateMilestoneCommand
        {
            MilestoneId = Guid.NewGuid(), AcuerdoId = Guid.NewGuid(), UserId = "user",
            Titulo = "Test", ImporteParcial = 100
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
