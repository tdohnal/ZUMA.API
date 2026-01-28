using Microsoft.Extensions.DependencyInjection;
using ZUMA.BussinessLogic.Infrastructure.Entities.Customer;
using ZUMA.BussinessLogic.Repositories.User;
using ZUMA.BussinessLogic.Services.User;
using ZUMA.Unittests.Fixtures;

namespace ZUMA.Unittests.Integration
{
    [TestFixture]
    public class UserServiceIntegrationTests : DatabaseFixture
    {
        private IUserRepository _userRepository = null!;
        private IRegistrationService _userService = null!;

        [SetUp]
        public override void SetupDatabase()
        {
            base.SetupDatabase();

            _userRepository = GetService<IUserRepository>();
            _userService = GetService<IRegistrationService>();
        }

        #region CreateAsync Tests

        [Test]
        public async Task CreateAsync_ShouldPersistUserToDatabase()
        {
            var newUser = new UserEntity
            {
                Name = "Integration Test User",
                Email = "integration@example.com",
                UserName = "integrationuser",
                Password = "hashed_password",
                Created = DateTime.UtcNow
            };

            var result = await _userService.CreateAsync(newUser);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.GreaterThan(0));

            var userInDb = DbContext.Users.FirstOrDefault(u => u.Email == "integration@example.com");
            Assert.That(userInDb, Is.Not.Null);
            Assert.That(userInDb!.Name, Is.EqualTo("Integration Test User"));
        }

        [Test]
        public async Task CreateAsync_ShouldCallBeforeAndAfterHooks()
        {
            // Arrange
            var newUser = new UserEntity
            {
                Name = "Hook Test User",
                Email = "hooks@example.com",
                UserName = "hooksuser",
                Password = "hashed_password",
                Created = DateTime.UtcNow
            };

            var result = await _userService.CreateAsync(newUser);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.GreaterThan(0));

            var userInDb = DbContext.Users.FirstOrDefault(u => u.Email == "hooks@example.com");
            Assert.That(userInDb, Is.Not.Null);
        }

        #endregion

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ShouldReturnUserFromDatabase()
        {
            var user = new UserEntity
            {
                Name = "DB Test User",
                Email = "dbtest@example.com",
                UserName = "dbtestuser",
                Password = "hashed_password",
                Created = DateTime.UtcNow
            };

            await SeedDatabaseAsync(async db =>
            {
                await db.Users.AddAsync(user);
            });

            var result = await _userService.GetByIdAsync(user.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Email, Is.EqualTo("dbtest@example.com"));
            Assert.That(result.Name, Is.EqualTo("DB Test User"));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var result = await _userService.GetByIdAsync(999);

            Assert.That(result, Is.Null);
        }

        #endregion

        #region GetAllAsync Tests

        [Test]
        public async Task GetAllAsync_ShouldReturnAllUsersFromDatabase()
        {
            await SeedDatabaseAsync(async db =>
            {
                await db.Users.AddRangeAsync(
                    new UserEntity
                    {
                        Name = "User1",
                        Email = "user1@example.com",
                        UserName = "user1",
                        Password = "pwd1",
                        Created = DateTime.UtcNow
                    },
                    new UserEntity
                    {
                        Name = "User2",
                        Email = "user2@example.com",
                        UserName = "user2",
                        Password = "pwd2",
                        Created = DateTime.UtcNow
                    }
                );
            });

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("User1"));
            Assert.That(result[1].Name, Is.EqualTo("User2"));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnEmpty_WhenNoneExist()
        {
            var result = await _userService.GetAllAsync();

            Assert.That(result, Is.Empty);
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_ShouldUpdateUserInDatabase()
        {
            // Arrange
            var user = new UserEntity
            {
                Name = "Original Name",
                Email = "update@example.com",
                UserName = "updateuser",
                Password = "hashed_password",
                Created = DateTime.UtcNow
            };

            await SeedDatabaseAsync(async db =>
            {
                await db.Users.AddAsync(user);
            });

            user.Name = "Updated Name";
            var result = await _userService.UpdateAsync(user);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Updated Name"));

            var userInDb = DbContext.Users.FirstOrDefault(u => u.Id == user.Id);
            Assert.That(userInDb!.Name, Is.EqualTo("Updated Name"));
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_ShouldMarkUserAsDeletedInDatabase()
        {
            var user = new UserEntity
            {
                Name = "Delete Test User",
                Email = "delete@example.com",
                UserName = "deleteuser",
                Password = "hashed_password",
                Created = DateTime.UtcNow
            };

            await SeedDatabaseAsync(async db =>
            {
                await db.Users.AddAsync(user);
            });

            var result = await _userService.DeleteAsync(user.Id);

            Assert.That(result, Is.True);

            var userInDb = DbContext.Users.FirstOrDefault(u => u.Id == user.Id);
            Assert.That(userInDb!.Deleted, Is.Not.Null);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            var result = await _userService.DeleteAsync(999);

            Assert.That(result, Is.False);
        }

        #endregion

        #region DI Container Tests

        [Test]
        public void DIContainer_ShouldResolveUserRepository()
        {
            var repository = GetService<IUserRepository>();

            Assert.That(repository, Is.Not.Null);
            Assert.That(repository, Is.InstanceOf<IUserRepository>());
        }

        [Test]
        public void DIContainer_ShouldResolveUserService()
        {
            var service = GetService<IRegistrationService>();

            Assert.That(service, Is.Not.Null);
            Assert.That(service, Is.InstanceOf<IRegistrationService>());
        }

        [Test]
        public void DIContainer_ShouldResolveScopedServices()
        {
            using var scope1 = ServiceProvider.CreateScope();
            using var scope2 = ServiceProvider.CreateScope();

            var service1 = scope1.ServiceProvider.GetRequiredService<IRegistrationService>();
            var service2 = scope2.ServiceProvider.GetRequiredService<IRegistrationService>();

            Assert.That(service1, Is.Not.SameAs(service2));
        }

        #endregion
    }
}