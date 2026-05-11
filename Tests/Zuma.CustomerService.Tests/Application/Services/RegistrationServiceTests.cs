using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Zuma.CustomerService.Tests.Application.Base;
using ZUMA.CustomerService.Application.Services;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Events;

namespace Zuma.CustomerService.Tests.Application.Services;

public class RegistrationServiceTests : GenericServiceUnitTests<RegistrationEntity, IRegistrationService, IRegistrationRepository>
{
    private Mock<IEventPublisherService> _eventMock = null!;
    private Mock<IUserService> _userServiceMock = null!;

    // Tato metoda běží dříve než konstruktor této třídy (voláno z base ctor)
    protected override IRegistrationService CreateService()
    {
        _eventMock = new Mock<IEventPublisherService>();
        _userServiceMock = new Mock<IUserService>();
        var classLogger = Mock.Of<ILogger<RegistrationService>>();

        return new RegistrationService(
            RepoMock.Object,
            _eventMock.Object,
            _userServiceMock.Object,
            classLogger);
    }

    protected override void MapRequiredProperties(RegistrationEntity entity)
    {
        // Ručně inicializujeme navigaci User, na které služba padá
        entity.User = new UserEntity
        {
            Email = "test@zuma.cz",
            UserName = "testuser"
        };
        // Případně další required pole pro registraci
        entity.ActivationCode = "TEMP";
    }

    [Fact]
    public async Task CreateAsync_ShouldGenerateActivationCode_AndCreateUser_AndPublishEvent()
    {
        // Arrange
        // Použijeme Fixture z báze
        var registration = Fixture.Create<RegistrationEntity>();
        registration.User.Email = "newuser@zuma.cz";

        _userServiceMock.Setup(s => s.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserEntity { Id = 500, Email = registration.User.Email });

        // RepoMock je definován v bázi
        RepoMock.Setup(r => r.CreateAsync(It.IsAny<RegistrationEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registration);

        // Act
        // SUT je IRegistrationService definovaný v bázi
        var result = await SUT.CreateAsync(registration);

        // Assert
        result.Should().NotBeNull();

        // 1. Kontrola logiky v RegistrationService (pravděpodobně v BeforeCreateAsync)
        result!.ActivationCode.Should().NotBeNullOrEmpty().And.HaveLength(10);
        result.ExpirationCodeDate.Should().BeAfter(DateTime.UtcNow);

        // 2. Kontrola volání UserService
        _userServiceMock.Verify(s => s.CreateAsync(
            It.Is<UserEntity>(u => u.Email == registration.User.Email),
            It.IsAny<CancellationToken>()), Times.Once);

        // 3. Kontrola odeslání eventu (pravděpodobně v AfterCreateAsync)
        _eventMock.Verify(e => e.PublishCreateEmailEventAsync(
            It.Is<CreateEmailEvent>(ev => ev.Subject == "Welcome in Zuma" && ev.Email == registration.User.Email),
            It.IsAny<CancellationToken>()), Times.Once);
    }

}