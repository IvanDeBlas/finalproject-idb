using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdfunding.Application.Features.Dashboard.Queries;
using WePlayRises.Crowdfunding.Application.Features.Dashboard.Validators;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Dashboard.Validators;

public class GetDashboardMisCampaniasQueryValidatorTests
{
    private readonly GetDashboardMisCampaniasQueryValidator _sut;

    public GetDashboardMisCampaniasQueryValidatorTests()
    {
        _sut = new GetDashboardMisCampaniasQueryValidator();
    }

    private static GetDashboardMisCampaniasQuery CreateValidQuery() => new()
    {
        UserId = "test-user-id",
        Page = 1,
        PageSize = 10,
        EstadoCampaniaId = null
    };

    [Fact]
    public async Task Validate_ValidQuery_ReturnsValid()
    {
        // Arrange
        var query = CreateValidQuery();

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ReturnsError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.UserId = "";

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_PageZero_ReturnsError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.Page = 0;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Fact]
    public async Task Validate_PageSizeTooLarge_ReturnsError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.PageSize = 101;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Fact]
    public async Task Validate_InvalidEstadoCampaniaId_ReturnsError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.EstadoCampaniaId = 6;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.EstadoCampaniaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Fact]
    public async Task Validate_NullEstadoCampaniaId_IsValid()
    {
        // Arrange
        var query = CreateValidQuery();
        query.EstadoCampaniaId = null;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task Validate_ValidEstadoCampaniaId_IsValid(int estadoId)
    {
        // Arrange
        var query = CreateValidQuery();
        query.EstadoCampaniaId = estadoId;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
