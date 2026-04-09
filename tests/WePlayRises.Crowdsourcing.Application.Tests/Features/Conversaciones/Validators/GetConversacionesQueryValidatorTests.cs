using FluentAssertions;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Queries;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Validators;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Conversaciones.Validators;

public class GetConversacionesQueryValidatorTests
{
    private readonly GetConversacionesQueryValidator _sut;

    public GetConversacionesQueryValidatorTests()
    {
        _sut = new GetConversacionesQueryValidator();
    }

    [Fact]
    public async Task Validate_ValidQuery_ReturnsValid()
    {
        var query = new GetConversacionesQuery { UserId = Guid.NewGuid().ToString(), Contexto = "todas", Page = 1, PageSize = 20 };
        var result = await _sut.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyUserId_ReturnsError()
    {
        var query = new GetConversacionesQuery { UserId = "", Contexto = "todas", Page = 1, PageSize = 20 };
        var result = await _sut.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_PageZero_ReturnsError()
    {
        var query = new GetConversacionesQuery { UserId = Guid.NewGuid().ToString(), Page = 0, PageSize = 20 };
        var result = await _sut.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Fact]
    public async Task Validate_PageSizeZero_ReturnsError()
    {
        var query = new GetConversacionesQuery { UserId = Guid.NewGuid().ToString(), Page = 1, PageSize = 0 };
        var result = await _sut.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorCode == ServiceResponseMessageType.Validation_InvalidRange);
    }

    [Fact]
    public async Task Validate_LargePageSize_ReturnsValid()
    {
        var query = new GetConversacionesQuery { UserId = Guid.NewGuid().ToString(), Page = 1, PageSize = 999 };
        var result = await _sut.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_InvalidContexto_ReturnsError()
    {
        var query = new GetConversacionesQuery { UserId = Guid.NewGuid().ToString(), Contexto = "invalid", Page = 1, PageSize = 20 };
        var result = await _sut.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_ContextoNecesidades_ReturnsValid()
    {
        var query = new GetConversacionesQuery { UserId = Guid.NewGuid().ToString(), Contexto = "necesidades", Page = 1, PageSize = 20 };
        var result = await _sut.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }
}
