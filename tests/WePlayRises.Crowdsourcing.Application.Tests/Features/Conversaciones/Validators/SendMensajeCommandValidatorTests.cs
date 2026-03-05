using FluentAssertions;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Validators;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Conversaciones.Validators;

public class SendMensajeCommandValidatorTests
{
    private readonly SendMensajeCommandValidator _sut;

    public SendMensajeCommandValidatorTests()
    {
        _sut = new SendMensajeCommandValidator();
    }

    private SendMensajeCommand CreateValidCommand()
    {
        return new SendMensajeCommand
        {
            ConversacionId = Guid.NewGuid(),
            UserIdRemitente = Guid.NewGuid().ToString(),
            Contenido = "Hola, este es un mensaje de prueba",
            UrlAdjunto = null
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
    public async Task Validate_ValidWithUrl_ReturnsValid()
    {
        var command = CreateValidCommand();
        command.UrlAdjunto = "https://example.com/file.pdf";
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyContenido_ReturnsError()
    {
        var command = CreateValidCommand();
        command.Contenido = "";
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_ContenidoTooLong_ReturnsError()
    {
        var command = CreateValidCommand();
        command.Contenido = new string('A', 5001);
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_InvalidUrl_ReturnsError()
    {
        var command = CreateValidCommand();
        command.UrlAdjunto = "not-a-valid-url";
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_InvalidUrl);
    }

    [Fact]
    public async Task Validate_UrlTooLong_ReturnsError()
    {
        var command = CreateValidCommand();
        command.UrlAdjunto = "https://example.com/" + new string('a', 2030);
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_MaxLength);
    }
}
