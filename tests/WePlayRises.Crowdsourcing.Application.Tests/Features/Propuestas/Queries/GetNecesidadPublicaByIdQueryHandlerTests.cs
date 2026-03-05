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

public class GetNecesidadPublicaByIdQueryHandlerTests
{
    private readonly Mock<INecesidadCrowdsourcingService> _necesidadServiceMock;
    private readonly Mock<IPropuestaCrowdsourcingService> _propuestaServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IPerfilProfesionalService> _perfilServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetNecesidadPublicaByIdQueryHandler>> _loggerMock;
    private readonly GetNecesidadPublicaByIdQueryHandler _sut;

    public GetNecesidadPublicaByIdQueryHandlerTests()
    {
        _necesidadServiceMock = new Mock<INecesidadCrowdsourcingService>();
        _propuestaServiceMock = new Mock<IPropuestaCrowdsourcingService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _perfilServiceMock = new Mock<IPerfilProfesionalService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetNecesidadPublicaByIdQueryHandler>>();
        _sut = new GetNecesidadPublicaByIdQueryHandler(
            _necesidadServiceMock.Object,
            _propuestaServiceMock.Object,
            _artistaServiceMock.Object,
            _perfilServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingNecesidad_ReturnsDto()
    {
        // Arrange
        var necesidad = CrowdsourcingTestData.CreateNecesidadAbierta();
        var userId = Guid.NewGuid().ToString();
        var query = new GetNecesidadPublicaByIdQuery { Id = necesidad.Id.Value, UserId = userId };

        _necesidadServiceMock
            .Setup(s => s.GetPublicaByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);

        _propuestaServiceMock
            .Setup(s => s.ExistePropuestaActivaAsync(It.IsAny<NecesidadCrowdsourcingId>(), userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        _perfilServiceMock
            .Setup(s => s.ExistsByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _artistaServiceMock
            .Setup(s => s.GetByIdAsync(necesidad.ArtistaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Artista { Id = necesidad.ArtistaId, NombreArtistico = "Artist" });

        var dto = new NecesidadPublicaDto { Id = necesidad.Id.Value, Titulo = necesidad.Titulo };
        _mapperMock
            .Setup(m => m.Map<NecesidadPublicaDto>(necesidad))
            .Returns(dto);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.YaPropuso.Should().BeFalse();
        result.Data.EsPropietario.Should().BeFalse();
        result.Data.TienePerfilProfesional.Should().BeTrue();
        result.Data.Artista.Should().NotBeNull();
        result.Data.Artista.NombreArtistico.Should().Be("Artist");
    }

    [Fact]
    public async Task Handle_NecesidadNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = new GetNecesidadPublicaByIdQuery
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid().ToString()
        };

        _necesidadServiceMock
            .Setup(s => s.GetPublicaByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NecesidadCrowdsourcing?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Necesidad);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetNecesidadPublicaByIdQuery
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid().ToString()
        };

        _necesidadServiceMock
            .Setup(s => s.GetPublicaByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_UserIsOwner_SetsEsPropietarioTrue()
    {
        // Arrange
        var artistaId = new ArtistaId(Guid.NewGuid());
        var necesidad = CrowdsourcingTestData.CreateNecesidad(artistaId: artistaId.Value);
        var userId = Guid.NewGuid().ToString();
        var query = new GetNecesidadPublicaByIdQuery { Id = necesidad.Id.Value, UserId = userId };

        _necesidadServiceMock
            .Setup(s => s.GetPublicaByIdAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);

        _propuestaServiceMock
            .Setup(s => s.ExistePropuestaActivaAsync(It.IsAny<NecesidadCrowdsourcingId>(), userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Artista { Id = artistaId, NombreArtistico = "My Artist" });

        _perfilServiceMock
            .Setup(s => s.ExistsByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _artistaServiceMock
            .Setup(s => s.GetByIdAsync(necesidad.ArtistaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Artista { Id = artistaId, NombreArtistico = "My Artist" });

        var dto = new NecesidadPublicaDto { Id = necesidad.Id.Value, Titulo = necesidad.Titulo };
        _mapperMock
            .Setup(m => m.Map<NecesidadPublicaDto>(necesidad))
            .Returns(dto);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.EsPropietario.Should().BeTrue();
    }
}
