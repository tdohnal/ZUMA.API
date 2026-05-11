using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Zuma.CustomerService.Tests.Infrastructure.Persistance.Repositories.Base;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces; // Předpokládám, že zde je IRegistrationRepository
using ZUMA.CustomerService.Infrastructure.Repositories;

namespace Zuma.CustomerService.Tests.Infrastructure.Persistance.Repositories;

public class RegistrationRepositoryTests : GenericRepositoryTests<RegistrationEntity, IRegistrationRepository>
{
    protected override IRegistrationRepository CreateRepository(ILogger<IRegistrationRepository> logger)
    {
        var classLogger = Mock.Of<ILogger<RegistrationRepository>>();
        return new RegistrationRepository(classLogger, Context);
    }

    protected override void MapRequiredProperties(RegistrationEntity entity)
    {
        var user = new UserEntity { Id = 54, UserName = "Tester", Email = "a@a.cz" };
        Context.Users.Add(user);
        Context.SaveChanges();

        entity.UserId = 54;
        entity.ActivationCode = "12345";
    }

    protected override void UpdateProperties(RegistrationEntity entity)
    {
        entity.ActivationCode = "67890";
    }

    [Fact]
    public async Task ExistsByPublicId_ShouldWorkForRegistration()
    {
        var Registration = new RegistrationEntity { UserId = 54, ActivationCode = "12345" };
        await Repo.CreateAsync(Registration);

        var exists = await Repo.ExistsByPublicIdAsync(Registration.PublicId);

        exists.Should().BeTrue();
    }
}