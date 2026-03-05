using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Dashboard.Queries;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Dashboard.Queries;

public class GetDashboardMisCampaniasQueryHandlerTests
{
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<ICampaniaRepository> _campaniaRepoMock;
    private readonly Mock<IPedidoRepository> _pedidoRepoMock;
    private readonly Mock<IValidator<GetDashboardMisCampaniasQuery>> _validatorMock;
    private readonly Mock<ILogger<GetDashboardMisCampaniasQueryHandler>> _loggerMock;
    private readonly GetDashboardMisCampaniasQueryHandler _sut;

    public GetDashboardMisCampaniasQueryHandlerTests()
    {
        _artistaServiceMock = new Mock<IArtistaService>();
        _campaniaRepoMock = new Mock<ICampaniaRepository>();
        _pedidoRepoMock = new Mock<IPedidoRepository>();
        _validatorMock = new Mock<IValidator<GetDashboardMisCampaniasQuery>>();
        _loggerMock = new Mock<ILogger<GetDashboardMisCampaniasQueryHandler>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetDashboardMisCampaniasQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _sut = new GetDashboardMisCampaniasQueryHandler(
            _artistaServiceMock.Object,
            _campaniaRepoMock.Object,
            _pedidoRepoMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsPaginatedCampanias()
    {
        // Arrange
        var query = new GetDashboardMisCampaniasQuery
        {
            UserId = DashboardTestData.TestUserId,
            Page = 1,
            PageSize = 10
        };

        var artista = DashboardTestData.CreateArtista();
        var campanias = new List<CampaniaCrowdfunding>
        {
            DashboardTestData.CreateCampania(estadoId: 2, importePledged: 2000m)
        };

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);

        _campaniaRepoMock
            .Setup(r => r.GetMisCampaniasPaginatedAsync(artista.Id, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(campanias);

        _campaniaRepoMock
            .Setup(r => r.CountMisCampaniasAsync(artista.Id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _pedidoRepoMock
            .Setup(r => r.CountByCampaniaIdAsync(It.IsAny<CampaniaCrowdfundingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.TotalCount.Should().Be(1);
        result.Data.Items[0].NumBackers.Should().Be(5);
        result.Data.Items[0].EstadoCampaniaNombre.Should().Be("Publicada");
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = new GetDashboardMisCampaniasQuery { UserId = "unknown", Page = 1, PageSize = 10 };

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artista?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Artista);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var query = new GetDashboardMisCampaniasQuery { UserId = "", Page = 0, PageSize = 200 };

        _validatorMock
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("UserId", "UserId es requerido") { ErrorCode = ServiceResponseMessageType.Validation_Required }
            }));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m =>
            m.ErrorCode == ServiceResponseMessageType.Validation_Required);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var query = new GetDashboardMisCampaniasQuery { UserId = DashboardTestData.TestUserId, Page = 1, PageSize = 10 };

        _artistaServiceMock
            .Setup(s => s.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
