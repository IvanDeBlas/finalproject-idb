using FluentAssertions;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Validators;

public class RechazarPropuestaCommandValidatorTests
{
    private readonly Mock<IPropuestaCrowdsourcingService> _propuestaServiceMock;
    private readonly RechazarPropuestaCommandValidator _sut;

    public RechazarPropuestaCommandValidatorTests()
    {
        _propuestaServiceMock = new Mock<IPropuestaCrowdsourcingService>();
        _sut = new RechazarPropuestaCommandValidator(_propuestaServiceMock.Object);
    }

    private void SetupValidAsyncRules()
    {
        var propuesta = CrowdsourcingTestData.CreatePropuestaPendiente();
        _propuestaServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propuesta);
    }

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        // Arrange
        var command = new RechazarPropuestaCommand
        {
            PropuestaId = Guid.NewGuid(),
            UserId = "user-1",
            Motivo = "No encaja con el proyecto"
        };
        SetupValidAsyncRules();

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyPropuestaId_Fails()
    {
        // Arrange
        var command = new RechazarPropuestaCommand
        {
            PropuestaId = Guid.Empty,
            UserId = "user-1"
        };
        SetupValidAsyncRules();

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_MotivoTooLong_Fails()
    {
        // Arrange
        var command = new RechazarPropuestaCommand
        {
            PropuestaId = Guid.NewGuid(),
            UserId = "user-1",
            Motivo = new string('x', 501)
        };
        SetupValidAsyncRules();

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_PropuestaNotFound_Fails()
    {
        // Arrange
        var command = new RechazarPropuestaCommand
        {
            PropuestaId = Guid.NewGuid(),
            UserId = "user-1"
        };
        _propuestaServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<PropuestaCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<PropuestaCrowdsourcing?>(null));

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.NotFound_Propuesta);
    }
}
