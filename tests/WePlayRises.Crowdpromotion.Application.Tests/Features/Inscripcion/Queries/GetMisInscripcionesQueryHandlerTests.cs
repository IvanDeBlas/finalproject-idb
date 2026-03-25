using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Queries;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Mapping;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Inscripcion.Queries;

public class GetMisInscripcionesQueryHandlerTests
{
    private readonly Mock<IInscripcionService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<GetMisInscripcionesQueryHandler>> _loggerMock;
    private readonly GetMisInscripcionesQueryHandler _sut;

    public GetMisInscripcionesQueryHandlerTests()
    {
        _serviceMock = new Mock<IInscripcionService>();
        _loggerMock = new Mock<ILogger<GetMisInscripcionesQueryHandler>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InscripcionProfile>());
        _mapper = config.CreateMapper();

        _sut = new GetMisInscripcionesQueryHandler(
            _serviceMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccess()
    {
        // Arrange
        var query = new GetMisInscripcionesQuery
        {
            UserId = InscripcionTestData.DefaultUserId,
            Page = 1,
            PageSize = 10
        };
        var promotor = InscripcionTestData.CreateValidPromotor();
        var inscripciones = new List<PromoProgramaPromotor>
        {
            InscripcionTestData.CreateAprobadaInscripcion()
        };

        _serviceMock.Setup(s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock.Setup(s => s.GetMisInscripcionesAsync(
                It.IsAny<PromotorId>(), query.Page, query.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync((inscripciones as IReadOnlyList<PromoProgramaPromotor>, 1));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalCount.Should().Be(1);
        result.Data.Page.Should().Be(1);
        result.Data.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_PromotorNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = new GetMisInscripcionesQuery
        {
            UserId = InscripcionTestData.DefaultUserId,
            Page = 1,
            PageSize = 10
        };

        _serviceMock.Setup(s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Promotor?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Promotor);
    }

    [Fact]
    public async Task Handle_EmptyResults_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetMisInscripcionesQuery
        {
            UserId = InscripcionTestData.DefaultUserId,
            Page = 1,
            PageSize = 10
        };
        var promotor = InscripcionTestData.CreateValidPromotor();

        _serviceMock.Setup(s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock.Setup(s => s.GetMisInscripcionesAsync(
                It.IsAny<PromotorId>(), query.Page, query.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<PromoProgramaPromotor>() as IReadOnlyList<PromoProgramaPromotor>, 0));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalCount.Should().Be(0);
        result.Data.Items.Should().BeEmpty();
        result.Data.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalServerError()
    {
        // Arrange
        var query = new GetMisInscripcionesQuery
        {
            UserId = InscripcionTestData.DefaultUserId,
            Page = 1,
            PageSize = 10
        };

        _serviceMock.Setup(s => s.GetPromotorByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
