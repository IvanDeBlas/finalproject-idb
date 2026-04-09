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
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Commands;

public class DeleteMilestoneCommandHandlerTests
{
    private readonly Mock<IAcuerdoCrowdsourcingService> _acuerdoServiceMock;
    private readonly Mock<IAcuerdoCrowdsourcingMilestoneService> _milestoneServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IValidator<DeleteMilestoneCommand>> _validatorMock;
    private readonly Mock<ILogger<DeleteMilestoneCommandHandler>> _loggerMock;
    private readonly DeleteMilestoneCommandHandler _sut;

    public DeleteMilestoneCommandHandlerTests()
    {
        _acuerdoServiceMock = new Mock<IAcuerdoCrowdsourcingService>();
        _milestoneServiceMock = new Mock<IAcuerdoCrowdsourcingMilestoneService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _validatorMock = new Mock<IValidator<DeleteMilestoneCommand>>();
        _loggerMock = new Mock<ILogger<DeleteMilestoneCommandHandler>>();
        _sut = new DeleteMilestoneCommandHandler(
            _acuerdoServiceMock.Object, _milestoneServiceMock.Object,
            _artistaServiceMock.Object, _validatorMock.Object, _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<DeleteMilestoneCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidCommand_DeletesMilestone()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(artistaId, "user-artista");
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(artistaId);
        var milestone = CrowdsourcingTestData.CreateMilestone(acuerdo.Id);

        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-artista", It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);
        _milestoneServiceMock.Setup(s => s.GetByIdAsync(milestone.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(milestone);
        _milestoneServiceMock.Setup(s => s.TieneEntregablesAsync(milestone.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Handle(new DeleteMilestoneCommand
        {
            MilestoneId = milestone.Id, AcuerdoId = acuerdo.Id.Value, UserId = "user-artista"
        }, CancellationToken.None);

        // Assert
        result.Data.Should().BeTrue();
        _milestoneServiceMock.Verify(s => s.DeleteAsync(milestone.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MilestoneHasEntregables_ReturnsConflict()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(artistaId, "user-artista");
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(artistaId);
        var milestone = CrowdsourcingTestData.CreateMilestone(acuerdo.Id);

        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-artista", It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);
        _milestoneServiceMock.Setup(s => s.GetByIdAsync(milestone.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(milestone);
        _milestoneServiceMock.Setup(s => s.TieneEntregablesAsync(milestone.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(new DeleteMilestoneCommand
        {
            MilestoneId = milestone.Id, AcuerdoId = acuerdo.Id.Value, UserId = "user-artista"
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_MilestoneHasEntregables);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(artistaId, "user-artista");
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(artistaId);
        var milestone = CrowdsourcingTestData.CreateMilestone(acuerdo.Id);

        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-artista", It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);
        _milestoneServiceMock.Setup(s => s.GetByIdAsync(milestone.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(milestone);
        _milestoneServiceMock.Setup(s => s.TieneEntregablesAsync(milestone.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _milestoneServiceMock.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(new DeleteMilestoneCommand
        {
            MilestoneId = milestone.Id, AcuerdoId = acuerdo.Id.Value, UserId = "user-artista"
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
