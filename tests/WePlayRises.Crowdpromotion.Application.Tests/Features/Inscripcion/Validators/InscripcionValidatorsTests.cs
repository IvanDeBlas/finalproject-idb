using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands;
using WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Validators;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Inscripcion.Validators;

public class SolicitarInscripcionCommandValidatorTests
{
    private readonly SolicitarInscripcionCommandValidator _sut;

    public SolicitarInscripcionCommandValidatorTests()
    {
        _sut = new SolicitarInscripcionCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = InscripcionTestData.CreateSolicitarCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateSolicitarCommand(userId: "");

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyProgramaId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateSolicitarCommand(programaId: Guid.Empty);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProgramaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}

public class AprobarInscripcionCommandValidatorTests
{
    private readonly AprobarInscripcionCommandValidator _sut;

    public AprobarInscripcionCommandValidatorTests()
    {
        _sut = new AprobarInscripcionCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = InscripcionTestData.CreateAprobarCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateAprobarCommand(userId: "");

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyProgramaId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateAprobarCommand(programaId: Guid.Empty);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProgramaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyInscripcionId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateAprobarCommand(inscripcionId: Guid.Empty);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.InscripcionId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}

public class RechazarInscripcionCommandValidatorTests
{
    private readonly RechazarInscripcionCommandValidator _sut;

    public RechazarInscripcionCommandValidatorTests()
    {
        _sut = new RechazarInscripcionCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = InscripcionTestData.CreateRechazarCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateRechazarCommand(userId: "");

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyProgramaId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateRechazarCommand(programaId: Guid.Empty);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProgramaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyInscripcionId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateRechazarCommand(inscripcionId: Guid.Empty);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.InscripcionId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}

public class BloquearInscripcionCommandValidatorTests
{
    private readonly BloquearInscripcionCommandValidator _sut;

    public BloquearInscripcionCommandValidatorTests()
    {
        _sut = new BloquearInscripcionCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = InscripcionTestData.CreateBloquearCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateBloquearCommand(userId: "");

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyProgramaId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateBloquearCommand(programaId: Guid.Empty);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProgramaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyInscripcionId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateBloquearCommand(inscripcionId: Guid.Empty);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.InscripcionId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}

public class DarDeBajaInscripcionCommandValidatorTests
{
    private readonly DarDeBajaInscripcionCommandValidator _sut;

    public DarDeBajaInscripcionCommandValidatorTests()
    {
        _sut = new DarDeBajaInscripcionCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = InscripcionTestData.CreateDarDeBajaCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateDarDeBajaCommand(userId: "");

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyProgramaId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateDarDeBajaCommand(programaId: Guid.Empty);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProgramaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyInscripcionId_ShouldHaveError()
    {
        // Arrange
        var command = InscripcionTestData.CreateDarDeBajaCommand(inscripcionId: Guid.Empty);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.InscripcionId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
