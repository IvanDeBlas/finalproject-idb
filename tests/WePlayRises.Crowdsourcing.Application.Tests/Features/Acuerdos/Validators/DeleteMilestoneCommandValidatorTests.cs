using FluentAssertions;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Validators;

public class DeleteMilestoneCommandValidatorTests
{
    private readonly DeleteMilestoneCommandValidator _sut;

    public DeleteMilestoneCommandValidatorTests()
    {
        _sut = new DeleteMilestoneCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        var command = new DeleteMilestoneCommand
        {
            MilestoneId = Guid.NewGuid(),
            AcuerdoId = Guid.NewGuid(),
            UserId = "user-1"
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyMilestoneId_Fails()
    {
        var command = new DeleteMilestoneCommand
        {
            MilestoneId = Guid.Empty,
            AcuerdoId = Guid.NewGuid(),
            UserId = "user-1"
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyAcuerdoId_Fails()
    {
        var command = new DeleteMilestoneCommand
        {
            MilestoneId = Guid.NewGuid(),
            AcuerdoId = Guid.Empty,
            UserId = "user-1"
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }
}
