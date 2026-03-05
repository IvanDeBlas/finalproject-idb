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
        var command = InscripcionTestData.CreateSolicitarCommand();
        var result = await _sut.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateSolicitarCommand(userId: "");
        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyProgramaId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateSolicitarCommand(programaId: Guid.Empty);
        var result = await _sut.TestValidateAsync(command);
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
        var command = InscripcionTestData.CreateAprobarCommand();
        var result = await _sut.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateAprobarCommand(userId: "");
        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyProgramaId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateAprobarCommand(programaId: Guid.Empty);
        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.ProgramaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyInscripcionId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateAprobarCommand(inscripcionId: Guid.Empty);
        var result = await _sut.TestValidateAsync(command);
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
        var command = InscripcionTestData.CreateRechazarCommand();
        var result = await _sut.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateRechazarCommand(userId: "");
        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyProgramaId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateRechazarCommand(programaId: Guid.Empty);
        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.ProgramaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyInscripcionId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateRechazarCommand(inscripcionId: Guid.Empty);
        var result = await _sut.TestValidateAsync(command);
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
        var command = InscripcionTestData.CreateBloquearCommand();
        var result = await _sut.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateBloquearCommand(userId: "");
        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyProgramaId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateBloquearCommand(programaId: Guid.Empty);
        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.ProgramaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyInscripcionId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateBloquearCommand(inscripcionId: Guid.Empty);
        var result = await _sut.TestValidateAsync(command);
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
        var command = InscripcionTestData.CreateDarDeBajaCommand();
        var result = await _sut.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateDarDeBajaCommand(userId: "");
        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyProgramaId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateDarDeBajaCommand(programaId: Guid.Empty);
        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.ProgramaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyInscripcionId_ShouldHaveError()
    {
        var command = InscripcionTestData.CreateDarDeBajaCommand(inscripcionId: Guid.Empty);
        var result = await _sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.InscripcionId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
