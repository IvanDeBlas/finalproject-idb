using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdsourcing.Application.Features.Templates.Queries;
using WePlayRises.Crowdsourcing.Application.Features.Templates.Validators;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Templates.Validators;

public class GetPlantillaProyectoByIdValidatorTests
{
    private readonly GetPlantillaProyectoByIdValidator _sut;

    public GetPlantillaProyectoByIdValidatorTests()
    {
        _sut = new GetPlantillaProyectoByIdValidator();
    }

    [Fact]
    public async Task Validate_ValidId_ReturnsValid()
    {
        // Arrange
        var query = new GetPlantillaProyectoByIdQuery { Id = Guid.NewGuid() };

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_EmptyId_ReturnsError()
    {
        // Arrange
        var query = new GetPlantillaProyectoByIdQuery { Id = Guid.Empty };

        // Act
        var result = await _sut.TestValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
