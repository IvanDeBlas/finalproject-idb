using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Validators;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Wallet.Validators;

public class GetWalletTransaccionesQueryValidatorTests
{
    private readonly GetWalletTransaccionesQueryValidator _sut;

    public GetWalletTransaccionesQueryValidatorTests()
    {
        _sut = new GetWalletTransaccionesQueryValidator();
    }

    private static GetWalletTransaccionesQuery CreateValidQuery()
    {
        return new GetWalletTransaccionesQuery
        {
            UserId = Guid.NewGuid().ToString(),
            Page = 1,
            PageSize = 10
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
    public async Task Validate_PageZero_HasRangeError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.Page = 0;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);
    }

    [Fact]
    public async Task Validate_NegativePage_HasRangeError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.Page = -1;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);
    }

    [Fact]
    public async Task Validate_PageSizeZero_HasRangeError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.PageSize = 0;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);
    }

    [Fact]
    public async Task Validate_PageSizeOver50_HasRangeError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.PageSize = 51;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);
    }

    [Fact]
    public async Task Validate_PageSize50_IsValid()
    {
        // Arrange
        var query = CreateValidQuery();
        query.PageSize = 50;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task Validate_EstadoTransaccionIdOutOfRange_HasRangeError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.EstadoTransaccionId = 5;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EstadoTransaccionId)
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);
    }

    [Fact]
    public async Task Validate_EstadoTransaccionIdZero_HasRangeError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.EstadoTransaccionId = 0;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EstadoTransaccionId)
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);
    }

    [Fact]
    public async Task Validate_EstadoTransaccionIdNull_IsValid()
    {
        // Arrange
        var query = CreateValidQuery();
        query.EstadoTransaccionId = null;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.EstadoTransaccionId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task Validate_EstadoTransaccionIdInRange_IsValid(int estadoId)
    {
        // Arrange
        var query = CreateValidQuery();
        query.EstadoTransaccionId = estadoId;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.EstadoTransaccionId);
    }

    [Fact]
    public async Task Validate_FechaDesdeAfterFechaHasta_HasFechaRangoError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.FechaDesde = new DateTime(2026, 3, 15);
        query.FechaHasta = new DateTime(2026, 3, 1);

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FechaDesde)
            .WithErrorCode(ServiceResponseMessageType.Validation_FechaRangoInvalido);
    }

    [Fact]
    public async Task Validate_FechaDesdeBeforeFechaHasta_IsValid()
    {
        // Arrange
        var query = CreateValidQuery();
        query.FechaDesde = new DateTime(2026, 3, 1);
        query.FechaHasta = new DateTime(2026, 3, 15);

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FechaDesde);
    }

    [Fact]
    public async Task Validate_OnlyFechaDesde_IsValid()
    {
        // Arrange
        var query = CreateValidQuery();
        query.FechaDesde = new DateTime(2026, 3, 15);
        query.FechaHasta = null;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FechaDesde);
    }
}
