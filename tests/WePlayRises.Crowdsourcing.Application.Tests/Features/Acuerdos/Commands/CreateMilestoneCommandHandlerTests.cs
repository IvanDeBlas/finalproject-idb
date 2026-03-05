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

public class CreateMilestoneCommandHandlerTests
{
    private readonly Mock<IAcuerdoCrowdsourcingService> _acuerdoServiceMock;
    private readonly Mock<IAcuerdoCrowdsourcingMilestoneService> _milestoneServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IValidator<CreateMilestoneCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateMilestoneCommandHandler>> _loggerMock;
    private readonly CreateMilestoneCommandHandler _sut;

    public CreateMilestoneCommandHandlerTests()
    {
        _acuerdoServiceMock = new Mock<IAcuerdoCrowdsourcingService>();
        _milestoneServiceMock = new Mock<IAcuerdoCrowdsourcingMilestoneService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _validatorMock = new Mock<IValidator<CreateMilestoneCommand>>();
        _loggerMock = new Mock<ILogger<CreateMilestoneCommandHandler>>();
        _sut = new CreateMilestoneCommandHandler(
            _acuerdoServiceMock.Object,
            _milestoneServiceMock.Object,
            _artistaServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CreateMilestoneCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesMilestone()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(artistaId, "user-artista");
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(artistaId);
        var milestoneId = Guid.NewGuid();

        var command = new CreateMilestoneCommand
        {
            AcuerdoId = acuerdo.Id.Value, UserId = "user-artista",
            Titulo = "Fase 1", ImporteParcial = 300m
        };

        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-artista", It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);
        _milestoneServiceMock.Setup(s => s.CreateAsync(It.IsAny<AcuerdoCrowdsourcingMilestone>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(milestoneId);
        _milestoneServiceMock.Setup(s => s.GetSumaImportesAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(300m);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(milestoneId);
        result.Data.Titulo.Should().Be("Fase 1");
        result.Messages.Should().ContainSingle(m => m.HttpStatusCode == System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task Handle_AcuerdoNotActive_ReturnsConflict()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(artistaId, "user-artista");
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoCompletado(artistaId);

        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-artista", It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);

        // Act
        var result = await _sut.Handle(new CreateMilestoneCommand
        {
            AcuerdoId = acuerdo.Id.Value, UserId = "user-artista", Titulo = "Fase", ImporteParcial = 100
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_AcuerdoNotActive);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(artistaId, "user-artista");
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(artistaId);

        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-artista", It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);
        _milestoneServiceMock.Setup(s => s.CreateAsync(It.IsAny<AcuerdoCrowdsourcingMilestone>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(new CreateMilestoneCommand
        {
            AcuerdoId = acuerdo.Id.Value, UserId = "user-artista", Titulo = "Fase", ImporteParcial = 100
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
