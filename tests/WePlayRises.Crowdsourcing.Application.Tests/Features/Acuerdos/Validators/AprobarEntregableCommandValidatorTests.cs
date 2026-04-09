using FluentAssertions;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Validators;

public class AprobarEntregableCommandValidatorTests
{
    private readonly AprobarEntregableCommandValidator _sut;

    public AprobarEntregableCommandValidatorTests()
    {
        _sut = new AprobarEntregableCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_NullComentario_Passes()
    {
        var command = new AprobarEntregableCommand
        {
            EntregableId = Guid.NewGuid(),
            UserId = "user-1",
            Comentario = null
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ValidCommand_WithComentario_Passes()
    {
        var command = new AprobarEntregableCommand
        {
            EntregableId = Guid.NewGuid(),
            UserId = "user-1",
            Comentario = "Excelente trabajo, aprobado"
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ComentarioTooLong_Fails()
    {
        var command = new AprobarEntregableCommand
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
