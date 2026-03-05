using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Tests.Helpers;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Tests.Features.Promotor.Commands;

public class DesactivarPromotorCommandHandlerTests
{
    private readonly Mock<IPromotorService> _serviceMock;
    private readonly Mock<ILogger<DesactivarPromotorCommandHandler>> _loggerMock;
    private readonly DesactivarPromotorCommandHandler _sut;

    public DesactivarPromotorCommandHandlerTests()
    {
        _serviceMock = new Mock<IPromotorService>();
        _loggerMock = new Mock<ILogger<DesactivarPromotorCommandHandler>>();

        _sut = new DesactivarPromotorCommandHandler(
            _serviceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsDesactivadoResult()
    {
        // Arrange
        var userId = PromotorTestData.DefaultUserId;
        var command = new DesactivarPromotorCommand { UserId = userId };
        var promotor = PromotorTestData.CreateValidPromotor(userId: userId, esActivo: true);

        _serviceMock.Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        _serviceMock.Setup(s => s.DesactivarWithProgramasAsync(promotor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EsActivo.Should().BeFalse();
        result.Data.ProgramasDadosDeBaja.Should().Be(3);
        result.Messages.Should().Contain(m => m.HttpStatusCode == System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task Handle_PromotorNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = new DesactivarPromotorCommand { UserId = "unknown-user" };

        _serviceMock.Setup(s => s.GetByUserIdAsync("unknown-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Model.Promotor?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Promotor);
    }

    [Fact]
    public async Task Handle_AlreadyInactive_ReturnsBusinessRuleError()
    {
        // Arrange
        var userId = PromotorTestData.DefaultUserId;
        var command = new DesactivarPromotorCommand { UserId = userId };
        var promotor = PromotorTestData.CreateValidPromotor(userId: userId, esActivo: false);

        _serviceMock.Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotor);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m =>
            m.ErrorCode == ServiceResponseMessageType.BusinessRule_PromotorAlreadyInactive);
    }

    [Fact]
    public async Task Handle_NullUserId_ReturnsBadRequest()
    {
        // Arrange
        var command = new DesactivarPromotorCommand { UserId = null };

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
        var command = new DesactivarPromotorCommand { UserId = "user1" };

        _serviceMock.Setup(s => s.GetByUserIdAsync("user1", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Messages.Should().Contain(m => m.ErrorCode == ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
