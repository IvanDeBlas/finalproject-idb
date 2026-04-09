using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Mapping;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Promotor.Commands;

public class CreatePromotorCommandHandlerTests
{
    private readonly Mock<IPromotorService> _serviceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidator<CreatePromotorCommand>> _validatorMock;
    private readonly Mock<ILogger<CreatePromotorCommandHandler>> _loggerMock;
    private readonly CreatePromotorCommandHandler _sut;

    public CreatePromotorCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromotorService>();
        _validatorMock = new Mock<IValidator<CreatePromotorCommand>>();
        _loggerMock = new Mock<ILogger<CreatePromotorCommandHandler>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<PromotorProfile>());
        _mapper = config.CreateMapper();

        _sut = new CreatePromotorCommandHandler(
            _serviceMock.Object,
            _mapper,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCreatedResult()
    {
        // Arrange
        var command = PromotorTestData.CreateValidCommand();
        var promotorId = PromotorId.CreateNew();

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Model.Promotor?)null);

        _serviceMock.Setup(s => s.CreateWithWalletAsync(It.IsAny<Domain.Model.Promotor>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotorId);

        _serviceMock.Setup(s => s.GetByIdAsync(promotorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PromotorTestData.CreateValidPromotor(promotorId, command.UserId));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.NombrePublico.Should().Be("Test Promotor");
        result.Data.EsActivo.Should().BeTrue();
        result.Messages.Should().Contain(m => m.HttpStatusCode == System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var command = new CreatePromotorCommand { NombrePublico = "", TipoPromotorId = 0, UserId = "user1" };
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
        result.Messages.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_PromotorAlreadyExists_ReturnsBusinessRuleError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidCommand();
        var existingPromotor = PromotorTestData.CreateValidPromotor(userId: command.UserId);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPromotor);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_PromotorAlreadyExists);
    }

    [Fact]
    public async Task Handle_NullUserId_ReturnsBadRequest()
    {
        // Arrange
        var command = PromotorTestData.CreateValidCommand();
        command.UserId = null;

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Auth_Unauthorized);
    }

    [Fact]
    public async Task Handle_ServiceThrows_ReturnsInternalServerError()
    {
        // Arrange
        var command = PromotorTestData.CreateValidCommand();

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _serviceMock.Setup(s => s.GetByUserIdAsync(command.UserId!, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB connection failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m =>
            m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
