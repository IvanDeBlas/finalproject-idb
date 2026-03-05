using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Maestras.Queries;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Maestras.Queries;

public class GetRolesProfesionalesQueryHandlerTests
{
    private readonly Mock<IRolProfesionalService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetRolesProfesionalesQueryHandler>> _loggerMock;
    private readonly GetRolesProfesionalesQueryHandler _sut;

    public GetRolesProfesionalesQueryHandlerTests()
    {
        _serviceMock = new Mock<IRolProfesionalService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetRolesProfesionalesQueryHandler>>();
        _sut = new GetRolesProfesionalesQueryHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_RolesExist_ReturnsMappedDtos()
    {
        // Arrange
        var roles = CrowdsourcingTestData.CreateRolesProfesionales(3);
        var expectedDtos = roles.Select(r => new RolProfesionalConCategoriaDto
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Activo = r.Activo
        }).ToList();

        _serviceMock
            .Setup(s => s.GetAllActivosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);

        _mapperMock
            .Setup(m => m.Map<List<RolProfesionalConCategoriaDto>>(roles))
            .Returns(expectedDtos);

        var query = new GetRolesProfesionalesQuery();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_NoRoles_ReturnsEmptyList()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetAllActivosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MaestraRolProfesional>());

        _mapperMock
            .Setup(m => m.Map<List<RolProfesionalConCategoriaDto>>(It.IsAny<IReadOnlyList<MaestraRolProfesional>>()))
            .Returns(new List<RolProfesionalConCategoriaDto>());

        var query = new GetRolesProfesionalesQuery();

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
            .ThrowsAsync(new Exception("DB error"));

        var query = new GetRolesProfesionalesQuery();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
