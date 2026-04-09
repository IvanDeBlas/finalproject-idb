using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Mapping;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Promotor.Commands;

public class UpdatePromotorCommandHandlerTests
{
    private readonly Mock<IPromotorService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<UpdatePromotorCommand>> _validatorMock;
    private readonly Mock<ILogger<UpdatePromotorCommandHandler>> _loggerMock;
    private readonly UpdatePromotorCommandHandler _sut;

    public UpdatePromotorCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromotorService>();
        _validatorMock = new Mock<IValidator<UpdatePromotorCommand>>();
        _loggerMock = new Mock<ILogger<UpdatePromotorCommandHandler>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<PromotorProfile>());
        _mapper = config.CreateMapper();

        _sut = new UpdatePromotorCommandHandler(
            _serviceMock.Object,
            _mapper,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedResult()
    {
        // Arrange
        var command = PromotorTestData.CreateValidUpdateCommand();
        var promotor = PromotorTestData.CreateValidPromotor(userId: command.UserId);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Domain.Model.Promotor>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.NombrePublico.Should().Be("DJ Marketing Pro Updated");
        result.Messages.Should().Contain(m => m.HttpStatusCode == System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task Handle_PromotorNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = PromotorTestData.CreateValidUpdateCommand();

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Model.Promotor?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Promotor);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsErrors()
    {
        // Arrange
        var command = new UpdatePromotorCommand { NombrePublico = "", UserId = "user1" };
        var failures = new List<ValidationFailure>
        {
            new("NombrePublico", "El nombre publico es obligatorio") { ErrorCode = ServiceResponseMessageType.Validation_Required }
        };

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalServerError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidUpdateCommand();

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
