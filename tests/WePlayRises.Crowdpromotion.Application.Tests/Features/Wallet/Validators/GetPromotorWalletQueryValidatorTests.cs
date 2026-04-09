using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Validators;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Wallet.Validators;

public class GetPromotorWalletQueryValidatorTests
{
    private readonly GetPromotorWalletQueryValidator _sut;

    public GetPromotorWalletQueryValidatorTests()
    {
        _sut = new GetPromotorWalletQueryValidator();
    }

    [Fact]
    public async Task Validate_ValidQuery_IsValid()
    {
        // Arrange
        var query = new GetPromotorWalletQuery { UserId = Guid.NewGuid().ToString() };

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_HasRequiredError()
    {
        // Arrange
        var query = new GetPromotorWalletQuery { UserId = string.Empty };

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_NullUserId_HasRequiredError()
    {
        // Arrange
        var query = new GetPromotorWalletQuery { UserId = null! };

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
