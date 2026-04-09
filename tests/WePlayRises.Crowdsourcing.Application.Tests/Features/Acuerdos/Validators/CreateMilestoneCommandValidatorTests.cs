using FluentAssertions;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Validators;

public class CreateMilestoneCommandValidatorTests
{
    private readonly Mock<IAcuerdoCrowdsourcingService> _acuerdoServiceMock;
    private readonly Mock<IAcuerdoCrowdsourcingMilestoneService> _milestoneServiceMock;
    private readonly CreateMilestoneCommandValidator _sut;

    public CreateMilestoneCommandValidatorTests()
    {
        _acuerdoServiceMock = new Mock<IAcuerdoCrowdsourcingService>();
        _milestoneServiceMock = new Mock<IAcuerdoCrowdsourcingMilestoneService>();
        _sut = new CreateMilestoneCommandValidator(
            _acuerdoServiceMock.Object,
            _milestoneServiceMock.Object);
    }

    private CreateMilestoneCommand CreateValidCommand()
    {
        return new CreateMilestoneCommand
        {
            AcuerdoId = Guid.NewGuid(),
            UserId = "user-1",
            Titulo = "Fase 1 - Mezcla",
            ImporteParcial = 300m
        };
    }

    private void SetupValidAsyncRules()
    {
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _milestoneServiceMock.Setup(s => s.GetSumaImportesAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);
    }

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidAsyncRules();

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyTitulo_Fails()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Titulo = "";
        SetupValidAsyncRules();

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_TituloTooShort_Fails()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Titulo = "ab";
        SetupValidAsyncRules();

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_MinLength);
    }

    [Fact]
    public async Task Validate_ImporteZero_Fails()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImporteParcial = 0;
        SetupValidAsyncRules();

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_SumaExceedsTotal_Fails()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImporteParcial = 800m;
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _milestoneServiceMock.Setup(s => s.GetSumaImportesAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(500m); // 500 + 800 > 1000

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.BusinessRule_MilestoneImporteExceeded);
    }
}
