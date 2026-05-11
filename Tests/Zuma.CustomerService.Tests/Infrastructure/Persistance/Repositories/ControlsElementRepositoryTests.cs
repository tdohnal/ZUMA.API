using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Zuma.CustomerService.Tests.Infrastructure.Persistance.Repositories.Base;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces; // Předpokládám, že zde je IControlsElementRepository
using ZUMA.CustomerService.Infrastructure.Repositories;

namespace Zuma.CustomerService.Tests.Infrastructure.Persistance.Repositories;

public class ControlsElementRepositoryTests : GenericRepositoryTests<ControlsElementEntity, IControlsElementRepository>
{
    protected override IControlsElementRepository CreateRepository(ILogger<IControlsElementRepository> logger)
    {
        var classLogger = Mock.Of<ILogger<ControlsElementRepository>>();
        return new ControlsElementRepository(classLogger, Context);
    }

    protected override void MapRequiredProperties(ControlsElementEntity entity)
    {
        var user = new UserEntity { Id = 54, UserName = "Tester", Email = "a@a.cz" };
        Context.Users.Add(user);
        Context.SaveChanges();

        entity.ListType = ZUMA.SharedKernel.Domain.Enums.ListType.CustomList;
        entity.OwnerUserId = 54;
        entity.Title = "Test Element";
    }

    protected override void UpdateProperties(ControlsElementEntity entity)
    {
        entity.Title = "Updated Test Element";
    }

    [Fact]
    public async Task ExistsByPublicId_ShouldWorkForControlsElement()
    {
        var ControlsElement = new ControlsElementEntity { ListType = ZUMA.SharedKernel.Domain.Enums.ListType.CustomList, OwnerUserId = 54, Title = "Test Element" };
        await Repo.CreateAsync(ControlsElement);

        var exists = await Repo.ExistsByPublicIdAsync(ControlsElement.PublicId);

        exists.Should().BeTrue();
    }
}