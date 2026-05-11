using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Zuma.CustomerService.Tests.Application.Base;
using ZUMA.CustomerService.Application.Services;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Events;

namespace Zuma.CustomerService.Tests.Application.Services;

public class UserServiceTests : GenericServiceUnitTests<UserEntity, IUserService, IUserRepository>
{
    // Tady nepoužíváme inicializaci v konstruktoru, ale definujeme je jako privátní pole, 
    // která naplníme v jedné metodě.
    private Mock<IEventPublisherService> _eventPublisherMock = null!;
    private Mock<IConfiguration> _configMock = null!;

    // Tato metoda se v bázovém konstruktoru zavolá jako PRVNÍ
    protected override IUserService CreateService()
    {
        // Inicializujeme mocky zde, protože tahle metoda běží dříve než konstruktor UserServiceTests
        _eventPublisherMock = new Mock<IEventPublisherService>();
        _configMock = new Mock<IConfiguration>();
        var classLogger = Mock.Of<ILogger<UserService>>();

        _configMock.Setup(c => c["Jwt:Key"]).Returns("SuperTajnyKlicProZumaApi1234567890");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("ZumaIssuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("ZumaAudience");

        return new UserService(
            RepoMock.Object,
            _eventPublisherMock.Object,
            classLogger,
            _configMock.Object);
    }

    public UserServiceTests() : base()
    {
    }

    #region VerificateAuthorizationCode Tests

    [Fact]
    public async Task VerificateAuthorizationCode_ValidCode_ReturnsToken()
    {
        // Arrange
        var email = "test@zuma.cz";
        var code = "123456";
        var user = Fixture.Build<UserEntity>()
            .With(u => u.Email, email)
            .With(u => u.AuthCode, code)
            .With(u => u.AuthCodeExpiration, DateTime.UtcNow.AddMinutes(5))
            .Create();

        RepoMock.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await SUT.VerificateAuthorizationCode(code, email);

        // Assert
        result.User.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        user.AuthCode.Should().BeNull();
        RepoMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("WRONG", "Invalid authorization code")]
    [InlineData("EXPIRED", "Authorization code expired")]
    public async Task VerificateAuthorizationCode_InvalidOrExpired_ReturnsErrorMessage(string scenario, string expectedError)
    {
        // Arrange
        var email = "test@zuma.cz";
        var inputCode = "123456";
        var user = Fixture.Build<UserEntity>()
            .With(u => u.Email, email)
            .With(u => u.AuthCode, scenario == "WRONG" ? "654321" : inputCode)
            .With(u => u.AuthCodeExpiration, scenario == "EXPIRED" ? DateTime.UtcNow.AddMinutes(-5) : DateTime.UtcNow.AddMinutes(5))
            .Create();

        RepoMock.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await SUT.VerificateAuthorizationCode(inputCode, email);

        // Assert
        result.ErrorMessage.Should().Be(expectedError);
        result.Token.Should().BeNull();
    }

    #endregion

    #region GetAuthorizationCode Tests

    [Fact]
    public async Task GetAuthorizationCodeAsync_UserExists_GeneratesCodeAndPublishesEvent()
    {
        // Arrange
        var userId = 1L;
        var user = Fixture.Build<UserEntity>().With(u => u.Id, userId).Create();

        RepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await SUT.GetAuthorizationCodeAsync(userId);

        // Assert
        user.AuthCode.Should().NotBeNullOrEmpty().And.HaveLength(6);
        RepoMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisherMock.Verify(e => e.PublishCreateEmailEventAsync(It.IsAny<CreateEmailEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}