using AutoFixture;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using ZUMA.CustomerService.Application.Consumers;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Authorization;

namespace Zuma.CustomerService.Tests.Application.Consumers;

public class AuthorizationConsumerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Fixture _fixture;

    public AuthorizationConsumerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
        _userServiceMock = new Mock<IUserService>();
    }

    #region VerifyCodeConsumer Tests

    [Fact]
    public async Task VerifyCodeConsumer_Success_ShouldRespondVerificationSuccess()
    {
        // Arrange
        var request = _fixture.Create<SendVerifyCodeRequest>();
        var user = _fixture.Create<UserEntity>();
        var contextMock = new Mock<ConsumeContext<SendVerifyCodeRequest>>();
        contextMock.Setup(x => x.Message).Returns(request);

        // Simulace úspěšného ověření
        var successResult = new VerificationResult { User = user, Token = "jwt-token" };
        _userServiceMock.Setup(s => s.VerificateAuthorizationCode(request.Code, request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        var consumer = new VerifyCodeConsumer(_userServiceMock.Object, new Mock<ILogger<VerifyCodeConsumer>>().Object);

        // Act
        await consumer.Consume(contextMock.Object);

        // Assert
        contextMock.Verify(x => x.RespondAsync(It.Is<VerificationSuccess>(r => r.Token == "jwt-token")), Times.Once);
    }

    [Fact]
    public async Task VerifyCodeConsumer_InvalidCode_ShouldRespondVerificationFailed()
    {
        // Arrange
        var request = _fixture.Create<SendVerifyCodeRequest>();
        var contextMock = new Mock<ConsumeContext<SendVerifyCodeRequest>>();
        contextMock.Setup(x => x.Message).Returns(request);

        // Simulace chyby (např. expirovaný kód)
        var failResult = new VerificationResult("Invalid authorization code");
        _userServiceMock.Setup(s => s.VerificateAuthorizationCode(request.Code, request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failResult);

        var consumer = new VerifyCodeConsumer(_userServiceMock.Object, new Mock<ILogger<VerifyCodeConsumer>>().Object);

        // Act
        await consumer.Consume(contextMock.Object);

        // Assert
        contextMock.Verify(x => x.RespondAsync<VerificationFailed>(It.Is<object>(obj => obj.ToString().Contains("Invalid"))), Times.Once);
    }

    #endregion

    #region AuthorizeUserConsumer Tests

    [Fact]
    public async Task AuthorizeUserConsumer_UserExists_ShouldRespondSuccess()
    {
        // Arrange
        var request = _fixture.Create<SendAuthorizeUserRequest>();
        var contextMock = new Mock<ConsumeContext<SendAuthorizeUserRequest>>();
        contextMock.Setup(x => x.Message).Returns(request);

        _userServiceMock.Setup(s => s.GetIdByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(123L);

        var consumer = new AuthorizeUserConsumer(_userServiceMock.Object, new Mock<ILogger<AuthorizeUserConsumer>>().Object);

        // Act
        await consumer.Consume(contextMock.Object);

        // Assert
        _userServiceMock.Verify(s => s.GetAuthorizationCodeAsync(123L, It.IsAny<CancellationToken>()), Times.Once);
        contextMock.Verify(x => x.RespondAsync(It.IsAny<AuthorizeUserSuccess>()), Times.Once);
    }

    [Fact]
    public async Task AuthorizeUserConsumer_UserNotFound_ShouldThrowAndTriggerFailed()
    {
        // Arrange
        var request = _fixture.Create<SendAuthorizeUserRequest>();
        var contextMock = new Mock<ConsumeContext<SendAuthorizeUserRequest>>();
        contextMock.Setup(x => x.Message).Returns(request);

        _userServiceMock.Setup(s => s.GetIdByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((long?)null);

        var consumer = new AuthorizeUserConsumer(_userServiceMock.Object, new Mock<ILogger<AuthorizeUserConsumer>>().Object);

        // Act & Assert
        // Tady máš v kódu 'throw new Exception', takže test musí čekat Exception
        var act = async () => await consumer.Consume(contextMock.Object);
        await act.Should().ThrowAsync<Exception>();

        // BaseConsumer v catch bloku zavolá OnFailedAsync -> RespondAsync<AuthorizeUserFailed>
        contextMock.Verify(x => x.RespondAsync<AuthorizeUserFailed>(It.IsAny<object>()), Times.Once);
    }

    #endregion
}