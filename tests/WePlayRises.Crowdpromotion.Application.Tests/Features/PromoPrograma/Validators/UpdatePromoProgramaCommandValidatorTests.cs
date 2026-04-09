using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands;
using WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Validators;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using Xunit;
using static WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands.UpdatePromoProgramaCommand;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.PromoPrograma.Validators;

public class UpdatePromoProgramaCommandValidatorTests
{
    private readonly Mock<IPromoProgramaService> _serviceMock;
    private readonly UpdatePromoProgramaCommandValidator _sut;

    public UpdatePromoProgramaCommandValidatorTests()
    {
        _serviceMock = new Mock<IPromoProgramaService>();

        // Happy path defaults: all foreign keys exist, no duplicates, no completados
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
        _serviceMock
            .Setup(s => s.TareaConCompletadosAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _sut = new UpdatePromoProgramaCommandValidator(_serviceMock.Object);
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidUpdateCommand();

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyId_ShouldHaveError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidUpdateCommand();
        command.Id = Guid.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_EmptyTitulo_ShouldHaveError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidUpdateCommand();
        command.Titulo = string.Empty;

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Titulo)
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Validate_InvalidCodigoTracking_ShouldHaveError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidUpdateCommand();
        command.CodigoTrackingBase = "promo@invalid!";

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CodigoTrackingBase)
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidCodigoTracking);
    }

    [Fact]
    public async Task Validate_DuplicateCodigoTracking_ShouldHaveError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidUpdateCommand();
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
    public async Task Validate_TareaConCompletadosDesactivar_ShouldHaveError()
    {
        // Arrange
        var tareaId = Guid.NewGuid();
        var command = PromoProgramaTestData.CreateValidUpdateCommand();
        command.Tareas = new List<UpdatePromoTareaItem>
        {
            new()
            {
                Id = tareaId,
                Titulo = "Tarea Con Completados",
                TipoEventoPromoId = 1,
                TipoRewardId = 2,
                PuntosRecompensa = 100,
                EsRepetible = false,
                EsActivo = false
            }
        };

        _serviceMock
            .Setup(s => s.TareaConCompletadosAsync(tareaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.Errors.Should().Contain(e =>
            e.ErrorCode == ServiceResponseMessageType.Validation_TareaConCompletados);
    }
}
