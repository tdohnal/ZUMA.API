using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Zuma.CustomerService.Tests.Infrastructure.Persistance.Base;
using ZUMA.SharedKernel.Domain.Interfaces;

namespace Zuma.CustomerService.Tests.Infrastructure.Persistance.Repositories.Base;

public abstract class GenericRepositoryTests<TEntity, TRepository> : PersistenceTestBase
    where TEntity : class, IAuditableEntities
    where TRepository : class, IRepositoryBase<TEntity>
{
    protected readonly TRepository Repo;
    protected readonly Mock<ILogger<TRepository>> LoggerMock;

    protected GenericRepositoryTests()
    {
        LoggerMock = new Mock<ILogger<TRepository>>();
        Repo = CreateRepository(LoggerMock.Object);
    }

    // Vrací interface (např. IUserRepository), který je public
    protected abstract TRepository CreateRepository(ILogger<TRepository> logger);

    // Specifické nastavení dat pro danou entitu
    protected abstract void MapRequiredProperties(TEntity entity);
    protected abstract void UpdateProperties(TEntity entity);

    [Fact]
    public virtual async Task FullLifecycle_ShouldVerifyBaseLogic()
    {
        // --- 1. CREATE ---
        var entity = (TEntity)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(TEntity));
        MapRequiredProperties(entity);

        var created = await Repo.CreateAsync(entity);

        created.Should().NotBeNull();
        created.Id.Should().BeGreaterThan(0);
        created.PublicId.Should().NotBe(Guid.Empty);
        created.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // --- 2. EXISTS & GET ---
        (await Repo.ExistsAsync(created.Id)).Should().BeTrue();

        var foundById = await Repo.GetByIdAsync(created.Id);
        foundById.Should().NotBeNull();

        var foundByPublicId = await Repo.GetByPublicIdAsync(created.PublicId);
        foundByPublicId.Should().NotBeNull();

        // --- 3. UPDATE ---
        UpdateProperties(foundById!);
        var updated = await Repo.UpdateAsync(foundById!);

        updated.Should().NotBeNull();
        updated!.Updated.Should().NotBeNull();
        updated.Updated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // --- 4. DELETE (Soft) ---
        bool deleteResult = await Repo.DeleteAsync(created.Id);
        deleteResult.Should().BeTrue();

        // --- 5. VERIFY SOFT DELETE ---
        (await Repo.GetByIdAsync(created.Id)).Should().BeNull();
        (await Repo.ExistsAsync(created.Id)).Should().BeFalse();

        // Raw DB check - ověření, že záznam v DB fyzicky existuje, ale má příznak Deleted
        var rawDb = await Context.Set<TEntity>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == created.Id);

        rawDb.Should().NotBeNull();
        rawDb!.Deleted.Should().NotBeNull();
        rawDb.Deleted.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}