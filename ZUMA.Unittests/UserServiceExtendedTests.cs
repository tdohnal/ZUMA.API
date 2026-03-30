using Microsoft.Extensions.Logging;
using Moq;
using ZUMA.BussinessLogic.Services;
using ZUMA.CustomerService.Entities;
using ZUMA.CustomerService.Repositories.User;
using ZUMA.CustomerService.Services.Messaging;
using ZUMA.CustomerService.Services.User;

namespace ZUMA.Unittests
{
    [TestFixture]
    public class UserServiceExtendedTests
    {
        private Mock<IUserRepository> _userRepoMock = null!;
        private Mock<IEventPublisherService> _eventPublisherMock = null!;
        private Mock<ILogger<UserService>> _loggerMock = null!;
        private Mock<IConfiguration> _configMock = null!;
        private IUserService _userService = null!;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _eventPublisherMock = new Mock<IEventPublisherService>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _configMock = new Mock<IConfiguration>();
            
            // Setup JWT config
            _configMock.Setup(x => x["Jwt:Key"]).Returns("your-secret-key-min-32-characters-long");
            _configMock.Setup(x => x["Jwt:Issuer"]).Returns("zuma");
            _configMock.Setup(x => x["Jwt:Audience"]).Returns("zuma");

            _userService = new UserService(
                _userRepoMock.Object,
                _eventPublisherMock.Object,
                _loggerMock.Object,
                _configMock.Object
            );
        }

        #region GetByPublicIdAsync Tests

        [Test]
        public async Task GetByPublicIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var publicId = Guid.NewGuid();
            var user = new UserEntity
            {
                Id = 1,
                PublicId = publicId,
                FullName = "Jan Novotný",
                Email = "jan@example.com",
                UserName = "jannovotny",
                Created = DateTime.UtcNow
            };

            _userRepoMock
                .Setup(r => r.GetByPublicIdAsync(publicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetByPublicIdAsync(publicId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.PublicId, Is.EqualTo(publicId));
            Assert.That(result.FullName, Is.EqualTo("Jan Novotný"));
            Assert.That(result.Email, Is.EqualTo("jan@example.com"));

            _userRepoMock.Verify(
                r => r.GetByPublicIdAsync(publicId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task GetByPublicIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var publicId = Guid.NewGuid();
            _userRepoMock
                .Setup(r => r.GetByPublicIdAsync(publicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            // Act
            var result = await _userService.GetByPublicIdAsync(publicId);

            // Assert
            Assert.That(result, Is.Null);

            _userRepoMock.Verify(
                r => r.GetByPublicIdAsync(publicId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region GetAllAsync Tests

        [Test]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<UserEntity>
            {
                new()
                {
                    Id = 1,
                    PublicId = Guid.NewGuid(),
                    FullName = "User One",
                    Email = "user1@example.com",
                    UserName = "user1",
                    Created = DateTime.UtcNow
                },
                new()
                {
                    Id = 2,
                    PublicId = Guid.NewGuid(),
                    FullName = "User Two",
                    Email = "user2@example.com",
                    UserName = "user2",
                    Created = DateTime.UtcNow
                }
            };

            _userRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].FullName, Is.EqualTo("User One"));
            Assert.That(result[1].FullName, Is.EqualTo("User Two"));

            _userRepoMock.Verify(
                r => r.GetAllAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            _userRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<UserEntity>());

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region CreateAsync Tests

        [Test]
        public async Task CreateAsync_ShouldCreateUser_WhenValidUserProvided()
        {
            // Arrange
            var newUser = new UserEntity
            {
                FullName = "Petr Nový",
                Email = "petr@example.com",
                UserName = "petrovy",
                Created = DateTime.UtcNow
            };

            var createdUser = new UserEntity
            {
                Id = 1,
                PublicId = Guid.NewGuid(),
                FullName = "Petr Nový",
                Email = "petr@example.com",
                UserName = "petrovy",
                Created = DateTime.UtcNow
            };

            _userRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _userService.CreateAsync(newUser);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.FullName, Is.EqualTo("Petr Nový"));
            Assert.That(result.Email, Is.EqualTo("petr@example.com"));

            _userRepoMock.Verify(
                r => r.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task CreateAsync_ShouldReturnNull_WhenCreationFails()
        {
            // Arrange
            var newUser = new UserEntity
            {
                FullName = "Test User",
                Email = "test@example.com",
                UserName = "testuser"
            };

            _userRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            // Act
            var result = await _userService.CreateAsync(newUser);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_ShouldUpdateUser_WhenValidUserProvided()
        {
            // Arrange
            var userToUpdate = new UserEntity
            {
                Id = 1,
                PublicId = Guid.NewGuid(),
                FullName = "Updated Name",
                Email = "updated@example.com",
                UserName = "updateduser",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            _userRepoMock
                .Setup(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userToUpdate);

            // Act
            var result = await _userService.UpdateAsync(userToUpdate);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.FullName, Is.EqualTo("Updated Name"));
            Assert.That(result.Email, Is.EqualTo("updated@example.com"));
            Assert.That(result.UserName, Is.EqualTo("updateduser"));

            _userRepoMock.Verify(
                r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ShouldReturnNull_WhenUpdateFails()
        {
            // Arrange
            var userToUpdate = new UserEntity
            {
                Id = 999,
                PublicId = Guid.NewGuid(),
                FullName = "Nonexistent User"
            };

            _userRepoMock
                .Setup(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            // Act
            var result = await _userService.UpdateAsync(userToUpdate);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            _userRepoMock
                .Setup(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.DeleteAsync(userId);

            // Assert
            Assert.That(result, Is.True);

            _userRepoMock.Verify(
                r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999;
            _userRepoMock
                .Setup(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _userService.DeleteAsync(userId);

            // Assert
            Assert.That(result, Is.False);

            _userRepoMock.Verify(
                r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region GetIdByEmailAsync Tests

        [Test]
        public async Task GetIdByEmailAsync_ShouldReturnUserId_WhenUserExists()
        {
            // Arrange
            var email = "test@example.com";
            var user = new UserEntity
            {
                Id = 42,
                PublicId = Guid.NewGuid(),
                Email = email,
                FullName = "Test User",
                UserName = "testuser"
            };

            _userRepoMock
                .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetIdByEmailAsync(email);

            // Assert
            Assert.That(result, Is.EqualTo(42));

            _userRepoMock.Verify(
                r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task GetIdByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _userRepoMock
                .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            // Act
            var result = await _userService.GetIdByEmailAsync(email);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetIdByEmailAsync_ShouldThrowArgumentNullException_WhenEmailIsNull()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _userService.GetIdByEmailAsync(null!));
        }

        #endregion

        #region Integration Tests

        [Test]
        public async Task CreateThenUpdate_ShouldWorkCorrectly()
        {
            // Arrange
            var newUser = new UserEntity
            {
                FullName = "Original Name",
                Email = "original@example.com",
                UserName = "originaluser"
            };

            var createdUser = new UserEntity
            {
                Id = 1,
                PublicId = Guid.NewGuid(),
                FullName = "Original Name",
                Email = "original@example.com",
                UserName = "originaluser",
                Created = DateTime.UtcNow
            };

            var updatedUser = new UserEntity
            {
                Id = 1,
                PublicId = createdUser.PublicId,
                FullName = "Updated Name",
                Email = "updated@example.com",
                UserName = "updateduser",
                Created = createdUser.Created,
                Updated = DateTime.UtcNow
            };

            _userRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdUser);

            _userRepoMock
                .Setup(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedUser);

            // Act - Create
            var created = await _userService.CreateAsync(newUser);
            Assert.That(created, Is.Not.Null);
            Assert.That(created!.FullName, Is.EqualTo("Original Name"));

            // Act - Update
            created.FullName = "Updated Name";
            created.Email = "updated@example.com";
            created.UserName = "updateduser";
            var updated = await _userService.UpdateAsync(created);

            // Assert
            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.FullName, Is.EqualTo("Updated Name"));
            Assert.That(updated.Email, Is.EqualTo("updated@example.com"));
        }

        [Test]
        public async Task CreateThenDelete_ShouldWorkCorrectly()
        {
            // Arrange
            var newUser = new UserEntity
            {
                FullName = "Test User",
                Email = "test@example.com",
                UserName = "testuser"
            };

            var createdUser = new UserEntity
            {
                Id = 5,
                PublicId = Guid.NewGuid(),
                FullName = "Test User",
                Email = "test@example.com",
                UserName = "testuser",
                Created = DateTime.UtcNow
            };

            _userRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdUser);

            _userRepoMock
                .Setup(r => r.DeleteAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act - Create
            var created = await _userService.CreateAsync(newUser);
            Assert.That(created, Is.Not.Null);

            // Act - Delete
            var deleted = await _userService.DeleteAsync(created!.Id);

            // Assert
            Assert.That(deleted, Is.True);
        }

        #endregion
    }
}
