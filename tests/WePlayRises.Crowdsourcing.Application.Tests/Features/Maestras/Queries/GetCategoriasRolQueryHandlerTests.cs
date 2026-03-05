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

public class GetCategoriasRolQueryHandlerTests
{
    private readonly Mock<ICategoriaRolService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetCategoriasRolQueryHandler>> _loggerMock;
    private readonly GetCategoriasRolQueryHandler _sut;

    public GetCategoriasRolQueryHandlerTests()
    {
        _serviceMock = new Mock<ICategoriaRolService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetCategoriasRolQueryHandler>>();
        _sut = new GetCategoriasRolQueryHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CategoriasExist_ReturnsMappedDtos()
    {
        // Arrange
        var categorias = CrowdsourcingTestData.CreateCategorias(3);
        var expectedDtos = categorias.Select(c => new CategoriaRolDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Orden = c.Orden
        }).ToList();

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorias);

        _mapperMock
            .Setup(m => m.Map<List<CategoriaRolDto>>(categorias))
            .Returns(expectedDtos);

        var query = new GetCategoriasRolQuery();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_NoCategorias_ReturnsEmptyList()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MaestraCategoriaRol>());

        _mapperMock
            .Setup(m => m.Map<List<CategoriaRolDto>>(It.IsAny<IReadOnlyList<MaestraCategoriaRol>>()))
            .Returns(new List<CategoriaRolDto>());

        var query = new GetCategoriasRolQuery();

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
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var query = new GetCategoriasRolQuery();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
