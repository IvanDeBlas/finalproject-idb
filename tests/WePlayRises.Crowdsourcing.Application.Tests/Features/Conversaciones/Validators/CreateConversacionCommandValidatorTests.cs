using FluentAssertions;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Validators;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Conversaciones.Validators;

public class CreateConversacionCommandValidatorTests
{
    private readonly CreateConversacionCommandValidator _sut;

    public CreateConversacionCommandValidatorTests()
    {
        _sut = new CreateConversacionCommandValidator();
    }

    private CreateConversacionCommand CreateValidCommand()
    {
        return new CreateConversacionCommand
        {
            UserIdCreador = Guid.NewGuid().ToString(),
            UserIdDestinatario = Guid.NewGuid().ToString(),
            Asunto = "Consulta sobre la necesidad",
            NecesidadId = Guid.NewGuid(),
            AcuerdoId = null
        };
    }

    [Fact]
    public async Task Validate_ValidCommand_ReturnsValid()
    {
        var command = CreateValidCommand();
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyAsunto_ReturnsError()
    {
        var command = CreateValidCommand();
        command.Asunto = "";
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_AsuntoTooLong_ReturnsError()
    {
        var command = CreateValidCommand();
        command.Asunto = new string('A', 201);
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_EmptyDestinatario_ReturnsError()
    {
        var command = CreateValidCommand();
        command.UserIdDestinatario = "";
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_BothNecesidadAndAcuerdo_ReturnsError()
    {
        var command = CreateValidCommand();
        command.NecesidadId = Guid.NewGuid();
        command.AcuerdoId = Guid.NewGuid();
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_NeitherNecesidadNorAcuerdo_ReturnsError()
    {
        var command = CreateValidCommand();
        command.NecesidadId = null;
        command.AcuerdoId = null;
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
    }
}
