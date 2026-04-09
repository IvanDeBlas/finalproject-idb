using FluentAssertions;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Validators;

public class CompletarAcuerdoCommandValidatorTests
{
    private readonly CompletarAcuerdoCommandValidator _sut;

    public CompletarAcuerdoCommandValidatorTests()
    {
        _sut = new CompletarAcuerdoCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        var command = new CompletarAcuerdoCommand
        {
            AcuerdoId = Guid.NewGuid(),
            UserId = "user-1"
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyAcuerdoId_Fails()
    {
        var command = new CompletarAcuerdoCommand
        {
            AcuerdoId = Guid.Empty,
            UserId = "user-1"
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }
}
