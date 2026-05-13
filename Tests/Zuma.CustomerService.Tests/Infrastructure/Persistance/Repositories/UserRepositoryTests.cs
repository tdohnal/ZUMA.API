using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Zuma.CustomerService.Tests.Infrastructure.Persistance.Repositories.Base;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces; // Předpokládám, že zde je IUserRepository
using ZUMA.CustomerService.Infrastructure.Repositories;

namespace Zuma.CustomerService.Tests.Infrastructure.Persistance.Repositories;

public class UserRepositoryTests : GenericRepositoryTests<UserEntity, IUserRepository>
{
    protected override IUserRepository CreateRepository(ILogger<IUserRepository> logger)
    {
        var classLogger = Mock.Of<ILogger<UserRepository>>();
        var distributedCache = Mock.Of<IDistributedCache>();
        return new UserRepository(Context, distributedCache, classLogger);
    }

    protected override void MapRequiredProperties(UserEntity entity)
    {
        entity.FullName = "Zuma Tester";
        entity.UserName = "ZumaTester";
        entity.Email = "tester@zuma.cz";
    }

    protected override void UpdateProperties(UserEntity entity)
    {
        entity.FullName = "Updated Name";
    }

    [Fact]
    public async Task GetByEmail_ShouldReturnUser()
    {
        var user = new UserEntity { FullName = "Zuma Tester", UserName = "SpecUser", Email = "spec@zuma.cz" };
        await Repo.CreateAsync(user);

        var found = await Repo.GetByEmailAsync(user.Email);

        found.Should().NotBeNull();
        found!.Id.Should().Be(user.Id);
    }
}