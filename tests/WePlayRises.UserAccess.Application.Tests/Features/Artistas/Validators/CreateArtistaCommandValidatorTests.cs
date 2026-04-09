using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.UserAccess.Application.Features.Artistas.Commands;
using WePlayRises.UserAccess.Application.Features.Artistas.Validators;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.Application.Tests.Features.Artistas.Validators;

public class CreateArtistaCommandValidatorTests
{
    private readonly CreateArtistaCommandValidator _sut;

    public CreateArtistaCommandValidatorTests()
    {
        _sut = new CreateArtistaCommandValidator();
    }

    private static CreateArtistaCommand CreateValidCommand() => new()
    {
        NombreArtistico = "Test Artist",
        Descripcion = "A great artist",
        Pais = "Spain",
        Ciudad = "Madrid",
        UserId = "user-123",
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
    public async Task Validate_EmptyNombreArtistico_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.NombreArtistico = "";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.NombreArtistico)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_NombreArtisticoTooLong_ReturnsMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.NombreArtistico = new string('A', 201);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.NombreArtistico)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_DescripcionTooLong_ReturnsMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Descripcion = new string('B', 2001);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Descripcion)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_PaisTooLong_ReturnsMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Pais = new string('C', 101);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Pais)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_CiudadTooLong_ReturnsMaxLengthError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Ciudad = new string('D', 101);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Ciudad)
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    [Fact]
    public async Task Validate_InvalidImagenUrl_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImagenUrl = "not-a-valid-url";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ImagenUrl)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl);
    }

    [Fact]
    public async Task Validate_ValidImagenUrl_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImagenUrl = "https://example.com/image.jpg";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_NullImagenUrl_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.ImagenUrl = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ReturnsError()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UserId = "";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_NullDescripcion_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Descripcion = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_NullPaisAndCiudad_ReturnsValid()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Pais = null;
        command.Ciudad = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
