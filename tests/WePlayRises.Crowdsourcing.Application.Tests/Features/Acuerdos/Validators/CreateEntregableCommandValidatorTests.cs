using FluentAssertions;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Validators;

public class CreateEntregableCommandValidatorTests
{
    private readonly CreateEntregableCommandValidator _sut;

    public CreateEntregableCommandValidatorTests()
    {
        _sut = new CreateEntregableCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        var command = new CreateEntregableCommand
        {
            AcuerdoId = Guid.NewGuid(),
            UserId = "user-1",
            Titulo = "Mezcla cancion 1",
            UrlRecurso = "https://drive.google.com/file/xyz"
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyTitulo_Fails()
    {
        var command = new CreateEntregableCommand
        {
            AcuerdoId = Guid.NewGuid(),
            UserId = "user-1",
            Titulo = ""
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_InvalidUrl_Fails()
    {
        var command = new CreateEntregableCommand
        {
            AcuerdoId = Guid.NewGuid(),
            UserId = "user-1",
            Titulo = "Entregable test",
            UrlRecurso = "not-a-valid-url"
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_InvalidUrl);
    }

    [Fact]
    public async Task Validate_NullUrl_Passes()
    {
        var command = new CreateEntregableCommand
        {
            AcuerdoId = Guid.NewGuid(),
            UserId = "user-1",
            Titulo = "Entregable test",
            UrlRecurso = null
        };

        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }
}
