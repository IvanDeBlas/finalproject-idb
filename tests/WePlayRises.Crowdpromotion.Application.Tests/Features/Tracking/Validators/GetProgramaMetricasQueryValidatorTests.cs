using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Queries;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Validators;
using WePlayRises.Crowdpromotion.Domain.Constants;
using Xunit;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Tracking.Validators;

public class GetProgramaMetricasQueryValidatorTests
{
    private readonly GetProgramaMetricasQueryValidator _sut;

    public GetProgramaMetricasQueryValidatorTests()
    {
        _sut = new GetProgramaMetricasQueryValidator();
    }

    private static GetProgramaMetricasQuery CreateValidQuery()
    {
        return new GetProgramaMetricasQuery
        {
            ProgramaId = Guid.NewGuid(),
            UserId = Guid.NewGuid().ToString(),
            FechaDesde = new DateOnly(2026, 1, 1),
            FechaHasta = new DateOnly(2026, 3, 1)
        };
    }

    [Fact]
    public async Task Validate_ValidQuery_IsValid()
    {
        // Arrange
        var query = CreateValidQuery();

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyProgramaId_HasRequiredError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.ProgramaId = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProgramaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyUserId_HasRequiredError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.UserId = string.Empty;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_FechaDesdeAfterFechaHasta_HasRangeError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.FechaDesde = new DateOnly(2026, 6, 1);
        query.FechaHasta = new DateOnly(2026, 1, 1);

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FechaDesde)
            .WithErrorCode(ServiceResponseMessageType.Validation_FechaRangoInvalido);
    }

    [Fact]
    public async Task Validate_FechaDesdeEqualToFechaHasta_IsValid()
    {
        // Arrange
        var query = CreateValidQuery();
        var sameDate = new DateOnly(2026, 2, 15);
        query.FechaDesde = sameDate;
        query.FechaHasta = sameDate;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FechaDesde);
    }

    [Fact]
    public async Task Validate_NullDates_IsValid()
    {
        // Arrange
        var query = CreateValidQuery();
        query.FechaDesde = null;
        query.FechaHasta = null;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_FechaDesdeNullWithFechaHasta_IsValid()
    {
        // Arrange
        var query = CreateValidQuery();
        query.FechaDesde = null;
        query.FechaHasta = new DateOnly(2026, 3, 1);

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FechaDesde);
    }

    [Fact]
    public async Task Validate_FechaDesdeWithNullFechaHasta_IsValid()
    {
        // Arrange
        var query = CreateValidQuery();
        query.FechaDesde = new DateOnly(2026, 1, 1);
        query.FechaHasta = null;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FechaDesde);
    }
}
