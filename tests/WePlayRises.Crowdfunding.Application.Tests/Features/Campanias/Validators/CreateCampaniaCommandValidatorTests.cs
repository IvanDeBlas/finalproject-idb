using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Validators;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Campanias.Validators;

public class CreateCampaniaCommandValidatorTests
{
    private readonly CreateCampaniaCommandValidator _sut;

    public CreateCampaniaCommandValidatorTests()
    {
        _sut = new CreateCampaniaCommandValidator();
    }

    private static CreateCampaniaCommand CreateValidCommand() => new()
    {
        ArtistaId = Guid.NewGuid(),
        Titulo = "Mi Campania de Crowdfunding",
        MonedaId = 1,
        ImporteObjetivo = 5000m,
        TipoFinanciacionId = 1,
        FechaInicio = DateTime.UtcNow.AddDays(1),
        FechaFin = DateTime.UtcNow.AddDays(30)
    };

    [Fact]
    public async Task Validate_ValidCommand_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_EmptyArtistaId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ArtistaId = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ArtistaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyTitulo_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Titulo = "";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Titulo)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_TituloTooLong_ReturnsMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Titulo = new string('A', 201);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Titulo)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_SubtituloTooLong_ReturnsMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Subtitulo = new string('B', 301);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Subtitulo)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_DescripcionCortaTooLong_ReturnsMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.DescripcionCorta = new string('C', 501);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.DescripcionCorta)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_InvalidVideoUrl_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.VideoPrincipalUrl = "not-a-valid-url";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.VideoPrincipalUrl)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl);
    }

    [Fact]
    public async Task Validate_ValidVideoUrl_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.VideoPrincipalUrl = "https://www.youtube.com/watch?v=12345";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ZeroMonedaId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.MonedaId = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.MonedaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_ZeroImporteObjetivo_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImporteObjetivo = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ImporteObjetivo)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);
    }

    [Fact]
    public async Task Validate_ImporteMinimoGreaterThanObjetivo_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImporteObjetivo = 5000m;
        command.ImporteMinimo = 10000m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ImporteMinimo)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Fact]
    public async Task Validate_ZeroTipoFinanciacionId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TipoFinanciacionId = 0;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.TipoFinanciacionId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_FechaFinBeforeFechaInicio_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.FechaInicio = DateTime.UtcNow.AddDays(30);
        command.FechaFin = DateTime.UtcNow.AddDays(10);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.FechaFin)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidDate);
    }
}
