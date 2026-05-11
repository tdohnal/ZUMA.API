using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ZUMA.SharedKernel.Domain.Interfaces;

namespace Zuma.CustomerService.Tests.Application.Base;

public abstract class GenericServiceUnitTests<TEntity, TService, TRepository>
    where TEntity : class, IAuditableEntities
    where TService : class, IServiceBase<TEntity>
    where TRepository : class, IRepositoryBase<TEntity>
{
    protected readonly Mock<TRepository> RepoMock;
    protected readonly Mock<ILogger<TService>> LoggerMock;
    protected readonly Fixture Fixture;
    protected readonly TService SUT;

    protected GenericServiceUnitTests()
    {
        Fixture = new Fixture();
        Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        RepoMock = new Mock<TRepository>();
        LoggerMock = new Mock<ILogger<TService>>();

        SUT = CreateService();
    }

    protected virtual void MapRequiredProperties(TEntity entity) { }

    protected abstract TService CreateService();

    [Fact]
    public virtual async Task CreateAsync_ShouldCallRepoAndReturnEntity()
    {
        // Arrange
        var entity = (TEntity)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(TEntity));

        // Tady zavoláme přípravu - pokud je to Registration, 
        // tak v odvozené třídě tady vyrobíme Usera.
        MapRequiredProperties(entity);

        RepoMock.Setup(r => r.CreateAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await SUT.CreateAsync(entity);

        // Assert
        result.Should().NotBeNull();
        RepoMock.Verify(r => r.CreateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public virtual async Task GetByIdAsync_ShouldReturnEntity()
    {
        // Arrange
        var entity = (TEntity)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(TEntity));
        RepoMock.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await SUT.GetByIdAsync(1L);

        // Assert
        result.Should().NotBeNull();
        RepoMock.Verify(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public virtual async Task DeleteAsync_ShouldReturnTrueOnSuccess()
    {
        // Arrange
        RepoMock.Setup(r => r.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await SUT.DeleteAsync(1L);

        // Assert
        result.Should().BeTrue();
        RepoMock.Verify(r => r.DeleteAsync(1L, It.IsAny<CancellationToken>()), Times.Once);
    }
}