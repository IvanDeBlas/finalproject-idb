using System.Net;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Mapping;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;
using PromoTareaEntity = WePlayRises.Crowdpromotion.Domain.Model.PromoTarea;
using Xunit;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.PromoPrograma.Commands;

public class CreatePromoProgramaCommandHandlerTests
{
    private readonly Mock<IPromoProgramaService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<CreatePromoProgramaCommand>> _validatorMock;
    private readonly Mock<ILogger<CreatePromoProgramaCommandHandler>> _loggerMock;
    private readonly CreatePromoProgramaCommandHandler _sut;

    public CreatePromoProgramaCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromoProgramaService>();
        _validatorMock = new Mock<IValidator<CreatePromoProgramaCommand>>();
        _loggerMock = new Mock<ILogger<CreatePromoProgramaCommandHandler>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PromoProgramaProfile>();
            cfg.AddProfile<PromoTareaProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _sut = new CreatePromoProgramaCommandHandler(
            _serviceMock.Object,
            _mapper,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCreatedResult()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();
        var artistaId = PromoProgramaTestData.DefaultArtistaId;
        var programaId = PromoProgramaId.CreateNew();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _serviceMock
            .Setup(s => s.CreateWithTareasAsync(
                It.IsAny<Domain.Model.PromoPrograma>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(programaId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(programaId.Value);
        result.Data.Titulo.Should().Be(command.Titulo);
        result.Data.EsActivo.Should().BeTrue();
        result.Data.TareasCreadas.Should().Be(command.Tareas.Count);
        result.Messages.Should().ContainSingle(m => m.HttpStatusCode == HttpStatusCode.Created);

        _serviceMock.Verify(
            s => s.CreateWithTareasAsync(
                It.IsAny<Domain.Model.PromoPrograma>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();

        var validationFailures = new List<ValidationFailure>
        {
            new("Titulo", "El titulo es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
            new("TipoPromoId", "El tipo de promo es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_ForeignKeyNotFound }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().NotBeEmpty();
        result.Messages.Should().HaveCount(2);

        _serviceMock.Verify(
            s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _serviceMock.Verify(
            s => s.CreateWithTareasAsync(
                It.IsAny<Domain.Model.PromoPrograma>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_NullUserId_ReturnsBadRequest()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();
        command.UserId = null;

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Unauthorized);

        _serviceMock.Verify(
            s => s.GetArtistaIdByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ArtistaNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArtistaId?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_Artista);

        _serviceMock.Verify(
            s => s.CreateWithTareasAsync(
                It.IsAny<Domain.Model.PromoPrograma>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();
        var artistaId = PromoProgramaTestData.DefaultArtistaId;

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _serviceMock
            .Setup(s => s.CreateWithTareasAsync(
                It.IsAny<Domain.Model.PromoPrograma>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }

    [Fact]
    public async Task Handle_ValidCommandWithMultipleTareas_MapsTareasCorrectly()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();
        command.Tareas.Add(PromoProgramaTestData.CreateValidTareaItem());
        command.Tareas.Add(PromoProgramaTestData.CreateValidTareaItem());

        var artistaId = PromoProgramaTestData.DefaultArtistaId;
        var programaId = PromoProgramaId.CreateNew();

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _serviceMock
            .Setup(s => s.CreateWithTareasAsync(
                It.IsAny<Domain.Model.PromoPrograma>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(programaId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.TareasCreadas.Should().Be(3);

        _serviceMock.Verify(
            s => s.CreateWithTareasAsync(
                It.IsAny<Domain.Model.PromoPrograma>(),
                It.Is<IReadOnlyList<PromoTareaEntity>>(t => t.Count == 3),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyUserId_ReturnsBadRequest()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidCommand();
        command.UserId = "";

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Unauthorized);
    }
}
