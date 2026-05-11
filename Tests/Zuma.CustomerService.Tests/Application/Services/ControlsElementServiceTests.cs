using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Zuma.CustomerService.Tests.Application.Base;
using ZUMA.CustomerService.Application.Services;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;

namespace Zuma.CustomerService.Tests.Application.Services;

public class ControlsElementServiceTests : GenericServiceUnitTests<ControlsElementEntity, IControlsElementService, IControlsElementRepository>
{
    private Mock<IEventPublisherService> _eventMock = null!;
    private Mock<IConfiguration> _configMock = null!;

    // Inicializace mocků dříve, než bázová třída vytvoří SUT
    protected override IControlsElementService CreateService()
    {
        _eventMock = new Mock<IEventPublisherService>();
        _configMock = new Mock<IConfiguration>();

        var classLogger = Mock.Of<ILogger<ControlsElementService>>();

        return new ControlsElementService(
            RepoMock.Object,
            _eventMock.Object,
            classLogger,
            _configMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldVerifyHooksAndRepoCall()
    {
        // Arrange
        var entity = Fixture.Build<ControlsElementEntity>()
            .Without(x => x.Items)
            .Create();

        RepoMock.Setup(r => r.CreateAsync(It.IsAny<ControlsElementEntity>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(entity);

        // Act
        var result = await SUT.CreateAsync(entity);

        // Assert
        result.Should().NotBeNull();
        // Ověření hooku ze ServiceBase v SharedKernelu
        result!.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        RepoMock.Verify(r => r.CreateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldConfirmSuccessfulExecution()
    {
        // Arrange
        long testId = 1;
        RepoMock.Setup(r => r.DeleteAsync(testId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

        // Act
        var result = await SUT.DeleteAsync(testId);

        // Assert
        result.Should().BeTrue();
        RepoMock.Verify(r => r.DeleteAsync(testId, It.IsAny<CancellationToken>()), Times.Once);
    }
}