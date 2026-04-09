using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Queries;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Necesidades.Queries;

public class GetNecesidadByIdQueryHandlerTests
{
    private readonly Mock<INecesidadCrowdsourcingService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetNecesidadByIdQueryHandler>> _loggerMock;
    private readonly GetNecesidadByIdQueryHandler _sut;

    public GetNecesidadByIdQueryHandlerTests()
    {
        _serviceMock = new Mock<INecesidadCrowdsourcingService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetNecesidadByIdQueryHandler>>();
        _sut = new GetNecesidadByIdQueryHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingNecesidad_OwnedByArtista_ReturnsMappedDto()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var necesidadId = Guid.NewGuid();
        var necesidad = CrowdsourcingTestData.CreateNecesidad(id: necesidadId, artistaId: artistaId);
        var expectedDto = new NecesidadCrowdsourcingDto { Id = necesidadId, Titulo = necesidad.Titulo };

        _serviceMock
            .Setup(s => s.GetByIdWithDetailsAsync(
                new NecesidadCrowdsourcingId(necesidadId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);

        _mapperMock
            .Setup(m => m.Map<NecesidadCrowdsourcingDto>(necesidad))
            .Returns(expectedDto);

        var query = new GetNecesidadByIdQuery { Id = necesidadId, ArtistaId = artistaId };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(necesidadId);
    }

    [Fact]
    public async Task Handle_NecesidadNotFound_ReturnsNotFound()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetByIdWithDetailsAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NecesidadCrowdsourcing?)null);

        var query = new GetNecesidadByIdQuery { Id = Guid.NewGuid(), ArtistaId = Guid.NewGuid() };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Necesidad);
    }

    [Fact]
    public async Task Handle_NecesidadOwnedByDifferentArtista_ReturnsForbidden()
    {
        // Arrange
        var ownerArtistaId = Guid.NewGuid();
        var requestingArtistaId = Guid.NewGuid();
        var necesidadId = Guid.NewGuid();
        var necesidad = CrowdsourcingTestData.CreateNecesidad(id: necesidadId, artistaId: ownerArtistaId);

        _serviceMock
            .Setup(s => s.GetByIdWithDetailsAsync(
                new NecesidadCrowdsourcingId(necesidadId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(necesidad);

        var query = new GetNecesidadByIdQuery { Id = necesidadId, ArtistaId = requestingArtistaId };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetNecesidadByIdQuery { Id = Guid.NewGuid(), ArtistaId = Guid.NewGuid() };

        _serviceMock
            .Setup(s => s.GetByIdWithDetailsAsync(It.IsAny<NecesidadCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
