using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Validators;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Campanias.Validators;

public class GetCampaniaStatsQueryValidatorTests
{
    private readonly GetCampaniaStatsQueryValidator _sut;

    public GetCampaniaStatsQueryValidatorTests()
    {
        _sut = new GetCampaniaStatsQueryValidator();
    }

    [Fact]
    public async Task Validate_ValidQuery_ReturnsValid()
    {
        // Arrange
        var query = new GetCampaniaStatsQuery { CampaniaId = Guid.NewGuid() };

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyCampaniaId_ReturnsError()
    {
        // Arrange
        var query = new GetCampaniaStatsQuery { CampaniaId = Guid.Empty };

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.CampaniaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
