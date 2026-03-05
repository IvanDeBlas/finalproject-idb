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

public class UpdatePromoProgramaCommandHandlerTests
{
    private readonly Mock<IPromoProgramaService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<UpdatePromoProgramaCommand>> _validatorMock;
    private readonly Mock<ILogger<UpdatePromoProgramaCommandHandler>> _loggerMock;
    private readonly UpdatePromoProgramaCommandHandler _sut;

    public UpdatePromoProgramaCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromoProgramaService>();
        _validatorMock = new Mock<IValidator<UpdatePromoProgramaCommand>>();
        _loggerMock = new Mock<ILogger<UpdatePromoProgramaCommandHandler>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PromoProgramaProfile>();
            cfg.AddProfile<PromoTareaProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _sut = new UpdatePromoProgramaCommandHandler(
            _serviceMock.Object,
            _mapper,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedResult()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidUpdateCommand();
        var artistaId = PromoProgramaTestData.DefaultArtistaId;
        var programa = PromoProgramaTestData.CreateValidPrograma(
            id: new PromoProgramaId(command.Id),
            artistaId: artistaId);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock
            .Setup(s => s.UpdateWithTareasAsync(
                It.IsAny<Domain.Model.PromoPrograma>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<IReadOnlyList<Guid>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(command.Id);
        result.Data.Titulo.Should().Be(command.Titulo);
        result.Data.FechaActualizacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Messages.Should().ContainSingle(m => m.HttpStatusCode == HttpStatusCode.OK);

        _serviceMock.Verify(
            s => s.UpdateWithTareasAsync(
                It.IsAny<Domain.Model.PromoPrograma>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<IReadOnlyList<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidUpdateCommand();

        var validationFailures = new List<ValidationFailure>
        {
            new("Titulo", "El titulo es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required },
            new("TipoPromoId", "El tipo de promo no existe") { ErrorCode = ServiceResponseMessageType.Validation_ForeignKeyNotFound }
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
            s => s.UpdateWithTareasAsync(
                It.IsAny<Domain.Model.PromoPrograma>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<IReadOnlyList<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ProgramaNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidUpdateCommand();
        var artistaId = PromoProgramaTestData.DefaultArtistaId;

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Model.PromoPrograma?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.NotFound_PromoPrograma);

        _serviceMock.Verify(
            s => s.UpdateWithTareasAsync(
                It.IsAny<Domain.Model.PromoPrograma>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<IReadOnlyList<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WrongArtista_ReturnsForbidden()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidUpdateCommand();
        var artistaId = PromoProgramaTestData.DefaultArtistaId;
        var differentArtistaId = new ArtistaId(Guid.NewGuid());

        var programa = PromoProgramaTestData.CreateValidPrograma(
            id: new PromoProgramaId(command.Id),
            artistaId: differentArtistaId);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m =>
            m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);

        _serviceMock.Verify(
            s => s.UpdateWithTareasAsync(
                It.IsAny<Domain.Model.PromoPrograma>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<IReadOnlyList<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidUpdateCommand();
        var artistaId = PromoProgramaTestData.DefaultArtistaId;
        var programa = PromoProgramaTestData.CreateValidPrograma(
            id: new PromoProgramaId(command.Id),
            artistaId: artistaId);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.GetArtistaIdByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artistaId);

        _serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(programa);

        _serviceMock
            .Setup(s => s.UpdateWithTareasAsync(
                It.IsAny<Domain.Model.PromoPrograma>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<IReadOnlyList<PromoTareaEntity>>(),
                It.IsAny<IReadOnlyList<Guid>>(),
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
    public async Task Handle_NullUserId_ReturnsBadRequest()
    {
        // Arrange
        var command = PromoProgramaTestData.CreateValidUpdateCommand();
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
        var command = PromoProgramaTestData.CreateValidUpdateCommand();

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
            s => s.GetByIdAsync(It.IsAny<PromoProgramaId>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
