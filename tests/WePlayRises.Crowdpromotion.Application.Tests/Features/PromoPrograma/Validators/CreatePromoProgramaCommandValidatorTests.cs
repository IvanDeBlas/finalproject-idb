using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands;
using WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Validators;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using Xunit;
using static WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands.CreatePromoProgramaCommand;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.PromoPrograma.Validators;

public class CreatePromoProgramaCommandValidatorTests
{
    private readonly Mock<IPromoProgramaService> _serviceMock;
    private readonly CreatePromoProgramaCommandValidator _sut;

    public CreatePromoProgramaCommandValidatorTests()
    {
        _serviceMock = new Mock<IPromoProgramaService>();

        // Happy path defaults: all foreign keys exist, no duplicates
        _serviceMock
            .Setup(s => s.TipoPromoExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _serviceMock
            .Setup(s => s.MonedaExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _serviceMock
            .Setup(s => s.TipoEventoPromoExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _serviceMock
            .Setup(s => s.TipoRewardExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _serviceMock
            .Setup(s => s.CodigoTrackingExistsAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _sut = new CreatePromoProgramaCommandValidator(_serviceMock.Object);
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyTitulo_ShouldHaveError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();
        command.Titulo = string.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Titulo)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_TituloTooShort_ShouldHaveError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();
        command.Titulo = "Ab";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Titulo)
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength);
    }

    [Fact]
    public async Task Validate_InvalidTipoPromo_ShouldHaveError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();
        _serviceMock
            .Setup(s => s.TipoPromoExistsAsync(command.TipoPromoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TipoPromoId)
            .WithErrorCode(ServiceResponseMessageType.Validation_ForeignKeyNotFound);
    }

    [Fact]
    public async Task Validate_NoComision_ShouldHaveError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();
        command.ImporteComisionPorcentaje = null;
        command.ImporteComisionFija = null;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_AtLeastOneComision);
    }

    [Fact]
    public async Task Validate_ComisionPorcentajeOutOfRange_ShouldHaveError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();
        command.ImporteComisionPorcentaje = 150m;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ImporteComisionPorcentaje)
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);
    }

    [Fact]
    public async Task Validate_DuplicateCodigoTracking_ShouldHaveError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();
        _serviceMock
            .Setup(s => s.CodigoTrackingExistsAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CodigoTrackingBase)
            .WithErrorCode(ServiceResponseMessageType.Validation_DuplicateCodigoTracking);
    }

    [Fact]
    public async Task Validate_TareaEsRepetibleWithoutMax_ShouldHaveError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();
        command.Tareas = new List<CreatePromoTareaItem>
        {
            new()
            {
                Titulo = "Tarea Repetible Sin Max",
                TipoEventoPromoId = 1,
                TipoRewardId = 2,
                PuntosRecompensa = 100,
                EsRepetible = true,
                MaxRepeticiones = null
            }
        };

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_EsRepetibleRequiresMax);
    }
}
