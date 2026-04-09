using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdfunding.Application.Features.Dashboard.Queries;
using WePlayRises.Crowdfunding.Application.Features.Dashboard.Validators;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Dashboard.Validators;

public class GetDashboardCampaniaBackingsQueryValidatorTests
{
    private readonly GetDashboardCampaniaBackingsQueryValidator _sut;

    public GetDashboardCampaniaBackingsQueryValidatorTests()
    {
        _sut = new GetDashboardCampaniaBackingsQueryValidator();
    }

    private static GetDashboardCampaniaBackingsQuery CreateValidQuery() => new()
    {
        CampaniaId = Guid.NewGuid(),
        UserId = "test-user-id",
        Page = 1,
        PageSize = 20
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
    public async Task Validate_EmptyCampaniaId_ReturnsError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.CampaniaId = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.CampaniaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
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
    public async Task Validate_PageSizeZero_ReturnsError()
    {
        // Arrange
        var query = CreateValidQuery();
        query.PageSize = 0;

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }
}
