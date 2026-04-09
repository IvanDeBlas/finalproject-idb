using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Validators;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Valoraciones.Validators;

public class CreateValoracionCommandValidatorTests
{
    private readonly CreateValoracionCommandValidator _sut;

    public CreateValoracionCommandValidatorTests()
    {
        _sut = new CreateValoracionCommandValidator();
    }

    private CreateValoracionCommand CreateValidCommand()
    {
        return new CreateValoracionCommand
        {
            AcuerdoId = Guid.NewGuid(),
            UserId = Guid.NewGuid().ToString(),
            Puntuacion = 5,
            Comentario = "Excelente trabajo"
        };
    }

    [Fact]
    public async Task Validate_ValidCommand_NoErrors()
    {
        var command = CreateValidCommand();
        var result = await _sut.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyAcuerdoId_HasError()
    {
        var command = CreateValidCommand();
        command.AcuerdoId = Guid.Empty;

        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.AcuerdoId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyUserId_HasError()
    {
        var command = CreateValidCommand();
        command.UserId = string.Empty;

        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_PuntuacionZero_HasError()
    {
        var command = CreateValidCommand();
        command.Puntuacion = 0;

        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Puntuacion)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(10)]
    public async Task Validate_PuntuacionOutOfRange_HasError(int puntuacion)
    {
        var command = CreateValidCommand();
        command.Puntuacion = puntuacion;

        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Puntuacion);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task Validate_PuntuacionInRange_NoError(int puntuacion)
    {
        var command = CreateValidCommand();
        command.Puntuacion = puntuacion;

        var result = await _sut.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Puntuacion);
    }

    [Fact]
    public async Task Validate_ComentarioOver1000Chars_HasError()
    {
        var command = CreateValidCommand();
        command.Comentario = new string('x', 1001);

        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Comentario)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_ComentarioExactly1000Chars_NoError()
    {
        var command = CreateValidCommand();
        command.Comentario = new string('x', 1000);

        var result = await _sut.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Comentario);
    }

    [Fact]
    public async Task Validate_NullComentario_NoError()
    {
        var command = CreateValidCommand();
        command.Comentario = null;

        var result = await _sut.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Comentario);
    }
}
