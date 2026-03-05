using FluentAssertions;
using FluentValidation.TestHelper;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Validators;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Campanias.Validators;

public class PublishCampaniaCommandValidatorTests
{
    private readonly PublishCampaniaCommandValidator _sut;

    public PublishCampaniaCommandValidatorTests()
    {
        _sut = new PublishCampaniaCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_ReturnsValid()
    {
        // Arrange
        var command = new PublishCampaniaCommand
        {
            Id = Guid.NewGuid(),
            ArtistaId = Guid.NewGuid()
        };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_EmptyId_ReturnsError()
    {
        // Arrange
        var command = new PublishCampaniaCommand
        {
            Id = Guid.Empty,
            ArtistaId = Guid.NewGuid()
        };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyArtistaId_ReturnsError()
    {
        // Arrange
        var command = new PublishCampaniaCommand
        {
            Id = Guid.NewGuid(),
            ArtistaId = Guid.Empty
        };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.ArtistaId)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
