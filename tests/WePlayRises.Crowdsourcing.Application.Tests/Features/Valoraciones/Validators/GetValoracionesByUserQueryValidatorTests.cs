using FluentValidation.TestHelper;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Queries;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Validators;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Valoraciones.Validators;

public class GetValoracionesByUserQueryValidatorTests
{
    private readonly GetValoracionesByUserQueryValidator _sut;

    public GetValoracionesByUserQueryValidatorTests()
    {
        _sut = new GetValoracionesByUserQueryValidator();
    }

    [Fact]
    public async Task Validate_ValidQuery_NoErrors()
    {
        var query = new GetValoracionesByUserQuery
        {
            UserId = Guid.NewGuid().ToString(),
            Page = 1,
            PageSize = 10
        };

        var result = await _sut.TestValidateAsync(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyUserId_HasError()
    {
        var query = new GetValoracionesByUserQuery
        {
            UserId = string.Empty,
            Page = 1,
            PageSize = 10
        };

        var result = await _sut.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_PageLessThan1_HasError(int page)
    {
        var query = new GetValoracionesByUserQuery
        {
            UserId = Guid.NewGuid().ToString(),
            Page = page,
            PageSize = 10
        };

        var result = await _sut.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(51)]
    [InlineData(100)]
    public async Task Validate_PageSizeOutOfRange_HasError(int pageSize)
    {
        var query = new GetValoracionesByUserQuery
        {
            UserId = Guid.NewGuid().ToString(),
            Page = 1,
            PageSize = pageSize
        };

        var result = await _sut.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(50)]
    public async Task Validate_PageSizeInRange_NoError(int pageSize)
    {
        var query = new GetValoracionesByUserQuery
        {
            UserId = Guid.NewGuid().ToString(),
            Page = 1,
            PageSize = pageSize
        };

        var result = await _sut.TestValidateAsync(query);
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }
}
