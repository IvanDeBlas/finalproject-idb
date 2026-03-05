using AutoMapper;
using FluentAssertions;
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

public class GetPlantillasProyectoQueryHandlerTests
{
    private readonly Mock<IPlantillaProyectoService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetPlantillasProyectoQueryHandler>> _loggerMock;
    private readonly GetPlantillasProyectoQueryHandler _sut;

    public GetPlantillasProyectoQueryHandlerTests()
    {
        _serviceMock = new Mock<IPlantillaProyectoService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetPlantillasProyectoQueryHandler>>();
        _sut = new GetPlantillasProyectoQueryHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_PlantillasExist_ReturnsMappedDtos()
    {
        // Arrange
        var plantilla1 = CrowdsourcingTestData.CreatePlantillaActiva();
        var plantilla2 = CrowdsourcingTestData.CreatePlantillaActiva();
        var plantillas = new List<PlantillaProyecto> { plantilla1, plantilla2 };

        var expectedDtos = new List<PlantillaProyectoListDto>
        {
            new() { Id = plantilla1.Id.Value, Nombre = plantilla1.Nombre },
            new() { Id = plantilla2.Id.Value, Nombre = plantilla2.Nombre }
        };

        _serviceMock
            .Setup(s => s.GetAllActivosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(plantillas);

        _serviceMock
            .Setup(s => s.GetByIdWithNecesidadesAsync(plantilla1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plantilla1);
        _serviceMock
            .Setup(s => s.GetByIdWithNecesidadesAsync(plantilla2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plantilla2);

        _mapperMock
            .Setup(m => m.Map<List<PlantillaProyectoListDto>>(It.IsAny<List<PlantillaProyecto>>()))
            .Returns(expectedDtos);

        var query = new GetPlantillasProyectoQuery();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_NoPlantillas_ReturnsEmptyList()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetAllActivosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlantillaProyecto>());

        _mapperMock
            .Setup(m => m.Map<List<PlantillaProyectoListDto>>(It.IsAny<List<PlantillaProyecto>>()))
            .Returns(new List<PlantillaProyectoListDto>());

        var query = new GetPlantillasProyectoQuery();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetAllActivosAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB connection lost"));

        var query = new GetPlantillasProyectoQuery();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
