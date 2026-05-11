using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Zuma.CustomerService.Tests.Infrastructure.Persistance.Repositories.Base;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces; // Předpokládám, že zde je IControlsElementRepository
using ZUMA.CustomerService.Infrastructure.Repositories;

namespace Zuma.CustomerService.Tests.Infrastructure.Persistance.Repositories;

public class ControlsElementsItemRepositoryTests : GenericRepositoryTests<ControlsElementsItemEntity, IControlsElementsItemRepository>
{
    protected override IControlsElementsItemRepository CreateRepository(ILogger<IControlsElementsItemRepository> logger)
    {
        var classLogger = Mock.Of<ILogger<ControlsElementsItemRepository>>();
        return new ControlsElementsItemRepository(classLogger, Context);
    }

    protected override void MapRequiredProperties(ControlsElementsItemEntity entity)
    {
        var user = new UserEntity { Id = 54, UserName = "Tester", Email = "a@a.cz" };
        Context.Users.Add(user);
        var controlsElement = new ControlsElementEntity { ListType = ZUMA.SharedKernel.Domain.Enums.ListType.CustomList, OwnerUserId = 54, Title = "Test Element" };
        Context.ControlElements.Add(controlsElement);
        Context.SaveChanges();

        entity.Content = "Test Content";
    }

    protected override void UpdateProperties(ControlsElementsItemEntity entity)
    {
        entity.Content = "Updated Test Content";
    }

    [Fact]
    public async Task ExistsByPublicId_ShouldWorkForControlsElement()
    {
        var controlsElement = new ControlsElementsItemEntity { Content = "Test Content" };
        await Repo.CreateAsync(controlsElement);

        var exists = await Repo.ExistsByPublicIdAsync(controlsElement.PublicId);

        exists.Should().BeTrue();
    }
}