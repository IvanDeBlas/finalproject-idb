using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdfunding.Application.Features.Dashboard.Queries;
using WePlayRises.Crowdfunding.Application.Features.Dashboard.Validators;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Dashboard.Validators;

public class GetDashboardCampaniaStatsQueryValidatorTests
{
    private readonly GetDashboardCampaniaStatsQueryValidator _sut;

    public GetDashboardCampaniaStatsQueryValidatorTests()
    {
        _sut = new GetDashboardCampaniaStatsQueryValidator();
    }

    private static GetDashboardCampaniaStatsQuery CreateValidQuery() => new()
    {
        CampaniaId = Guid.NewGuid(),
        UserId = "test-user-id"
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
    public async Task Validate_BothEmpty_ReturnsBothErrors()
    {
        // Arrange
        var query = new GetDashboardCampaniaStatsQuery
        {
            CampaniaId = Guid.Empty,
            UserId = ""
        };

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }
}
