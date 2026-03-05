using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Queries;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Queries;

public class GetAcuerdoByIdQueryHandlerTests
{
    private readonly Mock<IAcuerdoCrowdsourcingService> _acuerdoServiceMock;
    private readonly Mock<IArtistaService> _artistaServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetAcuerdoByIdQueryHandler>> _loggerMock;
    private readonly GetAcuerdoByIdQueryHandler _sut;

    public GetAcuerdoByIdQueryHandlerTests()
    {
        _acuerdoServiceMock = new Mock<IAcuerdoCrowdsourcingService>();
        _artistaServiceMock = new Mock<IArtistaService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetAcuerdoByIdQueryHandler>>();
        _sut = new GetAcuerdoByIdQueryHandler(
            _acuerdoServiceMock.Object, _artistaServiceMock.Object,
            _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AsArtista_ReturnsAcuerdoWithArtistaRol()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var artista = CrowdsourcingTestData.CreateArtista(artistaId, "user-artista");
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(artistaId);
        acuerdo.Necesidad = CrowdsourcingTestData.CreateNecesidadAbierta(artistaId);

        var dto = new AcuerdoDto { Milestones = new List<MilestoneDto>() };

        _acuerdoServiceMock.Setup(s => s.GetByIdWithDetailsAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-artista", It.IsAny<CancellationToken>()))
            .ReturnsAsync(artista);
        _mapperMock.Setup(m => m.Map<AcuerdoDto>(It.IsAny<AcuerdoCrowdsourcing>())).Returns(dto);

        // Act
        var result = await _sut.Handle(new GetAcuerdoByIdQuery
        {
            AcuerdoId = acuerdo.Id.Value, UserId = "user-artista"
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.MiRol.Should().Be("Artista");
    }

    [Fact]
    public async Task Handle_AsProfesional_ReturnsProfesionalRol()
    {
        // Arrange
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(userIdProveedor: "user-proveedor");
        acuerdo.Necesidad = CrowdsourcingTestData.CreateNecesidadAbierta();

        var dto = new AcuerdoDto { Milestones = new List<MilestoneDto>() };

        _acuerdoServiceMock.Setup(s => s.GetByIdWithDetailsAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _artistaServiceMock.Setup(s => s.GetByUserIdAsync("user-proveedor", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAccess.Domain.Model.Artista?)null);
        _mapperMock.Setup(m => m.Map<AcuerdoDto>(It.IsAny<AcuerdoCrowdsourcing>())).Returns(dto);

        // Act
        var result = await _sut.Handle(new GetAcuerdoByIdQuery
        {
            AcuerdoId = acuerdo.Id.Value, UserId = "user-proveedor"
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.MiRol.Should().Be("Profesional");
    }

    [Fact]
    public async Task Handle_AcuerdoNotFound_ReturnsNotFound()
    {
        // Arrange
        _acuerdoServiceMock.Setup(s => s.GetByIdWithDetailsAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcuerdoCrowdsourcing?)null);

        // Act
        var result = await _sut.Handle(new GetAcuerdoByIdQuery
        {
            AcuerdoId = Guid.NewGuid(), UserId = "user"
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Acuerdo);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        _acuerdoServiceMock.Setup(s => s.GetByIdWithDetailsAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(new GetAcuerdoByIdQuery
        {
            AcuerdoId = Guid.NewGuid(), UserId = "user"
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
