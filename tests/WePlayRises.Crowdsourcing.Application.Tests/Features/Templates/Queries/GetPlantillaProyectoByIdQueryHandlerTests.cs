using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Templates.Queries;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Templates.Queries;

public class GetPlantillaProyectoByIdQueryHandlerTests
{
    private readonly Mock<IPlantillaProyectoService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<GetPlantillaProyectoByIdQuery>> _validatorMock;
    private readonly Mock<ILogger<GetPlantillaProyectoByIdQueryHandler>> _loggerMock;
    private readonly GetPlantillaProyectoByIdQueryHandler _sut;

    public GetPlantillaProyectoByIdQueryHandlerTests()
    {
        _serviceMock = new Mock<IPlantillaProyectoService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<GetPlantillaProyectoByIdQuery>>();
        _loggerMock = new Mock<ILogger<GetPlantillaProyectoByIdQueryHandler>>();
        _sut = new GetPlantillaProyectoByIdQueryHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetPlantillaProyectoByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_PlantillaExists_ReturnsMappedDto()
    {
        // Arrange
        var plantillaId = Guid.NewGuid();
        var plantilla = CrowdsourcingTestData.CreatePlantillaActiva(plantillaId);
        var expectedDto = new PlantillaProyectoDto
        {
            Id = plantillaId,
            Nombre = plantilla.Nombre
        };

        SetupValidValidation();

        _serviceMock
            .Setup(s => s.GetByIdWithNecesidadesAsync(new PlantillaProyectoId(plantillaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plantilla);

        _mapperMock
            .Setup(m => m.Map<PlantillaProyectoDto>(plantilla))
            .Returns(expectedDto);

        var query = new GetPlantillaProyectoByIdQuery { Id = plantillaId };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(plantillaId);
    }

    [Fact]
    public async Task Handle_PlantillaNotFound_ReturnsNotFound()
    {
        // Arrange
        var plantillaId = Guid.NewGuid();
        SetupValidValidation();

        _serviceMock
            .Setup(s => s.GetByIdWithNecesidadesAsync(new PlantillaProyectoId(plantillaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlantillaProyecto?)null);

        var query = new GetPlantillaProyectoByIdQuery { Id = plantillaId };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_PlantillaProyecto);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new("Id", "El ID de la plantilla es obligatorio")
            {
                ErrorCode = ServiceResponseMessageType.Validation_Required
            }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetPlantillaProyectoByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var query = new GetPlantillaProyectoByIdQuery { Id = Guid.Empty };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Validation_Required);
        _serviceMock.Verify(s => s.GetByIdWithNecesidadesAsync(It.IsAny<PlantillaProyectoId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var plantillaId = Guid.NewGuid();
        SetupValidValidation();

        _serviceMock
            .Setup(s => s.GetByIdWithNecesidadesAsync(new PlantillaProyectoId(plantillaId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var query = new GetPlantillaProyectoByIdQuery { Id = plantillaId };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
