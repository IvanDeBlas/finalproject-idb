using FluentAssertions;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Validators;

public class RechazarEntregableCommandValidatorTests
{
    private readonly RechazarEntregableCommandValidator _sut;

    public RechazarEntregableCommandValidatorTests()
    {
        _sut = new RechazarEntregableCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        var command = new RechazarEntregableCommand
        {
            EntregableId = Guid.NewGuid(),
            UserId = "user-1",
            Comentario = "La calidad del audio necesita mejorar significativamente"
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyComentario_Fails()
    {
        var command = new RechazarEntregableCommand
        {
            EntregableId = Guid.NewGuid(),
            UserId = "user-1",
            Comentario = ""
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_ComentarioTooShort_Fails()
    {
        var command = new RechazarEntregableCommand
        {
            EntregableId = Guid.NewGuid(),
            UserId = "user-1",
            Comentario = "Malo"
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_MinLength);
    }

    [Fact]
    public async Task Validate_ComentarioTooLong_Fails()
    {
        var command = new RechazarEntregableCommand
        {
            EntregableId = Guid.NewGuid(),
            UserId = "user-1",
            Comentario = new string('x', 501)
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_MaxLength);
    }
}
