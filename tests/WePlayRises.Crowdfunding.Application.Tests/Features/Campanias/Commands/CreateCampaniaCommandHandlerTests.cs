using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Application.Tests.Helpers;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Features.Campanias.Commands;

public class CreateCampaniaCommandHandlerTests
{
    private readonly Mock<ICampaniaService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateCampaniaCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateCampaniaCommandHandler>> _loggerMock;
    private readonly CreateCampaniaCommandHandler _sut;

    public CreateCampaniaCommandHandlerTests()
    {
        _serviceMock = new Mock<ICampaniaService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreateCampaniaCommand>>();
        _loggerMock = new Mock<ILogger<CreateCampaniaCommandHandler>>();
        _sut = new CreateCampaniaCommandHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithCampaniaDto()
    {
        // Arrange
        var artistaId = Guid.NewGuid();
        var campaniaId = Guid.NewGuid();
        var command = new CreateCampaniaCommand
        {
            ArtistaId = artistaId,
            Titulo = "Mi Campania",
            MonedaId = 1,
            ImporteObjetivo = 5000m,
            TipoFinanciacionId = 1
        };

        var entity = CampaniaTestData.CreateValidBorrador(artistaId, campaniaId);
        var expectedDto = new CampaniaDto
        {
            Id = campaniaId,
            ArtistaId = artistaId,
            Titulo = "Mi Campania",
            ImporteObjetivo = 5000m
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mapperMock
            .Setup(m => m.Map<CampaniaCrowdfunding>(command))
            .Returns(entity);

        var returnedId = new CampaniaCrowdfundingId(campaniaId);
        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnedId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(returnedId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(m => m.Map<CampaniaDto>(entity))
            .Returns(expectedDto);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(campaniaId);
        result.Data.Titulo.Should().Be("Mi Campania");
        _serviceMock.Verify(s => s.CreateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = new CreateCampaniaCommand
        {
            ArtistaId = Guid.Empty,
            Titulo = "",
            MonedaId = 0,
            ImporteObjetivo = 0,
            TipoFinanciacionId = 0
        };

        var validationFailures = new List<ValidationFailure>
        {
            new("ArtistaId", "El ArtistaId es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
            new("Titulo", "El titulo es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().HaveCount(2);
        _serviceMock.Verify(s => s.CreateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = new CreateCampaniaCommand
        {
            ArtistaId = Guid.NewGuid(),
            Titulo = "Test Campania",
            MonedaId = 1,
            ImporteObjetivo = 5000m,
            TipoFinanciacionId = 1
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mapperMock
            .Setup(m => m.Map<CampaniaCrowdfunding>(command))
            .Returns(CampaniaTestData.CreateValidBorrador());

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<CampaniaCrowdfunding>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
