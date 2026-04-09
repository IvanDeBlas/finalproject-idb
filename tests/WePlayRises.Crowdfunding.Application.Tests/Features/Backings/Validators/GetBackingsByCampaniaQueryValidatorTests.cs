using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdfunding.Application.Features.Backings.Queries;
using WePlayRises.Crowdfunding.Application.Features.Backings.Validators;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Backings.Validators;

public class GetBackingsByCampaniaQueryValidatorTests
{
    private readonly GetBackingsByCampaniaQueryValidator _sut;

    public GetBackingsByCampaniaQueryValidatorTests()
    {
        _sut = new GetBackingsByCampaniaQueryValidator();
    }

    [Fact]
    public async Task Validate_ValidQuery_ReturnsValid()
    {
        // Arrange
        var query = new GetBackingsByCampaniaQuery
        {
            CampaniaId = Guid.NewGuid(),
            Limit = 20
        };

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyCampaniaId_ReturnsError()
    {
        // Arrange
        var query = new GetBackingsByCampaniaQuery
        {
            CampaniaId = Guid.Empty,
            Limit = 20
        };

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.CampaniaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_LimitZero_ReturnsError()
    {
        // Arrange
        var query = new GetBackingsByCampaniaQuery
        {
            CampaniaId = Guid.NewGuid(),
            Limit = 0
        };

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Limit)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Fact]
    public async Task Validate_LimitExceeds100_ReturnsError()
    {
        // Arrange
        var query = new GetBackingsByCampaniaQuery
        {
            CampaniaId = Guid.NewGuid(),
            Limit = 101
        };

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Limit)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task Validate_ValidLimitValues_ReturnsValid(int limit)
    {
        // Arrange
        var query = new GetBackingsByCampaniaQuery
        {
            CampaniaId = Guid.NewGuid(),
            Limit = limit
        };

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
