using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Campanias.Queries;

public class GetCampaniaByIdQueryHandlerTests
{
    private readonly Mock<ICampaniaService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetCampaniaByIdQueryHandler>> _loggerMock;
    private readonly GetCampaniaByIdQueryHandler _sut;

    public GetCampaniaByIdQueryHandlerTests()
    {
        _serviceMock = new Mock<ICampaniaService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetCampaniaByIdQueryHandler>>();
        _sut = new GetCampaniaByIdQueryHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CampaniaExists_ReturnsMappedDto()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var artistaId = Guid.NewGuid();
        var entity = CampaniaTestData.CreateValidBorrador(artistaId, campaniaId);

        var expectedDto = new CampaniaDto
        {
            Id = campaniaId,
            ArtistaId = artistaId,
            Titulo = entity.Titulo,
            ImporteObjetivo = entity.ImporteObjetivo
        };

        var query = new GetCampaniaByIdQuery { Id = campaniaId };

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(m => m.Map<CampaniaDto>(entity))
            .Returns(expectedDto);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(campaniaId);
        result.Data.Titulo.Should().Be(entity.Titulo);
    }

    [Fact]
    public async Task Handle_CampaniaNotFound_ReturnsNotFound()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var query = new GetCampaniaByIdQuery { Id = campaniaId };

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CampaniaCrowdfunding?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Campania);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var campaniaId = Guid.NewGuid();
        var query = new GetCampaniaByIdQuery { Id = campaniaId };

        _serviceMock
            .Setup(s => s.GetByIdAsync(new CampaniaCrowdfundingId(campaniaId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB connection lost"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
