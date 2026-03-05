using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Application.Tests.Helpers;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Features.Acuerdos.Commands;

public class CreateEntregableCommandHandlerTests
{
    private readonly Mock<IAcuerdoCrowdsourcingService> _acuerdoServiceMock;
    private readonly Mock<IAcuerdoCrowdsourcingMilestoneService> _milestoneServiceMock;
    private readonly Mock<IAcuerdoCrowdsourcingEntregableService> _entregableServiceMock;
    private readonly Mock<IValidator<CreateEntregableCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateEntregableCommandHandler>> _loggerMock;
    private readonly CreateEntregableCommandHandler _sut;

    public CreateEntregableCommandHandlerTests()
    {
        _acuerdoServiceMock = new Mock<IAcuerdoCrowdsourcingService>();
        _milestoneServiceMock = new Mock<IAcuerdoCrowdsourcingMilestoneService>();
        _entregableServiceMock = new Mock<IAcuerdoCrowdsourcingEntregableService>();
        _validatorMock = new Mock<IValidator<CreateEntregableCommand>>();
        _loggerMock = new Mock<ILogger<CreateEntregableCommandHandler>>();
        _sut = new CreateEntregableCommandHandler(
            _acuerdoServiceMock.Object,
            _milestoneServiceMock.Object,
            _entregableServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    private void SetupValidValidation()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CreateEntregableCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesEntregable()
    {
        // Arrange
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(userIdProveedor: "user-proveedor");
        var entregableId = Guid.NewGuid();

        var command = new CreateEntregableCommand
        {
            AcuerdoId = acuerdo.Id.Value, UserId = "user-proveedor",
            Titulo = "Entregable 1", UrlRecurso = "https://drive.google.com/file/test"
        };

        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _entregableServiceMock.Setup(s => s.CreateAsync(It.IsAny<AcuerdoCrowdsourcingEntregable>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entregableId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(entregableId);
        result.Data.EstadoEntregableNombre.Should().Be("Entregado");
        result.Messages.Should().ContainSingle(m => m.HttpStatusCode == System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task Handle_NotProveedor_ReturnsForbidden()
    {
        // Arrange
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(userIdProveedor: "user-proveedor");
        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);

        // Act
        var result = await _sut.Handle(new CreateEntregableCommand
        {
            AcuerdoId = acuerdo.Id.Value, UserId = "another-user", Titulo = "Test"
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalError()
    {
        // Arrange
        var acuerdo = CrowdsourcingTestData.CreateAcuerdoActivo(userIdProveedor: "user-proveedor");
        SetupValidValidation();
        _acuerdoServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<AcuerdoCrowdsourcingId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acuerdo);
        _entregableServiceMock.Setup(s => s.CreateAsync(It.IsAny<AcuerdoCrowdsourcingEntregable>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(new CreateEntregableCommand
        {
            AcuerdoId = acuerdo.Id.Value, UserId = "user-proveedor", Titulo = "Test"
        }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().ContainSingle(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
