using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Queries;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Propuestas.Queries;

public class GetMisPropuestasQueryHandlerTests
{
    private readonly Mock<IPropuestaCrowdsourcingService> _serviceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetMisPropuestasQueryHandler>> _loggerMock;
    private readonly GetMisPropuestasQueryHandler _sut;

    public GetMisPropuestasQueryHandlerTests()
    {
        _serviceMock = new Mock<IPropuestaCrowdsourcingService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetMisPropuestasQueryHandler>>();
        _sut = new GetMisPropuestasQueryHandler(
            _serviceMock.Object,
            _artistaServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithResults_ReturnsPaginatedResponse()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var query = new GetMisPropuestasQuery { UserId = userId, Page = 1, PageSize = 10 };

        var necesidad = CrowdsourcingTestData.CreateNecesidadAbierta();
        var propuesta = CrowdsourcingTestData.CreatePropuesta(necesidad.Id);
        propuesta.UserId = userId;
        propuesta.Necesidad = necesidad;

        var items = new List<PropuestaCrowdsourcing> { propuesta };

        _serviceMock
            .Setup(s => s.GetByUserIdPaginatedAsync(userId, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((items as IReadOnlyList<PropuestaCrowdsourcing>, 1));

        var dto = new MiPropuestaListDto { Id = propuesta.Id.Value, PrecioPropuesto = propuesta.PrecioPropuesto };
        _mapperMock
            .Setup(m => m.Map<List<MiPropuestaListDto>>(items))
            .Returns(new List<MiPropuestaListDto> { dto });

        _artistaServiceMock
            .Setup(s => s.GetByIdAsync(necesidad.ArtistaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Artista { Id = necesidad.ArtistaId, NombreArtistico = "Test Artist" });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_EmptyResults_ReturnsEmptyResponse()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var query = new GetMisPropuestasQuery { UserId = userId, Page = 1, PageSize = 10 };

        var emptyList = new List<PropuestaCrowdsourcing>();
        _serviceMock
            .Setup(s => s.GetByUserIdPaginatedAsync(userId, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((emptyList as IReadOnlyList<PropuestaCrowdsourcing>, 0));

        _mapperMock
            .Setup(m => m.Map<List<MiPropuestaListDto>>(emptyList))
            .Returns(new List<MiPropuestaListDto>());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetMisPropuestasQuery
        {
            UserId = Guid.NewGuid().ToString(),
            Page = 1,
            PageSize = 10
        };

        _serviceMock
            .Setup(s => s.GetByUserIdPaginatedAsync(
                It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
