using Microsoft.Extensions.Logging;
using Moq;
using ZUMA.BussinessLogic.Entities.Customer;
using ZUMA.BussinessLogic.Repositories.User;
using ZUMA.BussinessLogic.Services.User;

namespace ZUMA.Unittests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepoMock = null!;
        private Mock<ILogger<RegistrationService>> _loggerMock = null!;
        private IRegistrationService _userService = null!;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<RegistrationService>>();
            _userService = new UserService(_userRepoMock.Object, _loggerMock.Object);
        }

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var user = new UserEntity
            {
                Id = userId,
                Name = "Tom Ownski",
                Email = "tom@example.com",
                UserName = "tomown",
                Password = "hashed_password",
                Created = DateTime.UtcNow
            };

            _userRepoMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(userId));
            Assert.That(result.Name, Is.EqualTo("Tom Ownski"));
            Assert.That(result.Email, Is.EqualTo("tom@example.com"));

            _userRepoMock.Verify(
                r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999;
            _userRepoMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.That(result, Is.Null);

            _userRepoMock.Verify(
                r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()),
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
                    Name = "User1",
                    Email = "user1@example.com",
                    UserName = "user1",
                    Password = "pwd1",
                    Created = DateTime.UtcNow
                },
                new()
                {
                    Id = 2,
                    Name = "User2",
                    Email = "user2@example.com",
                    UserName = "user2",
                    Password = "pwd2",
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
            Assert.That(result[0].Name, Is.EqualTo("User1"));
            Assert.That(result[1].Name, Is.EqualTo("User2"));

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
                Name = "NewUser",
                Email = "newuser@example.com",
                UserName = "newuser",
                Password = "hashedpwd",
                Created = DateTime.UtcNow
            };

            var createdUser = new UserEntity
            {
                Id = 1,
                Name = "NewUser",
                Email = "newuser@example.com",
                UserName = "newuser",
                Password = "hashedpwd",
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
            Assert.That(result.Name, Is.EqualTo("NewUser"));

            _userRepoMock.Verify(
                r => r.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
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
                Name = "UpdatedName",
                Email = "updated@example.com",
                UserName = "updateduser",
                Password = "newhash",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            _userRepoMock
                .Setup(r => r.UpdateAsync(userToUpdate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userToUpdate);

            // Act
            var result = await _userService.UpdateAsync(userToUpdate);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("UpdatedName"));
            Assert.That(result.Email, Is.EqualTo("updated@example.com"));

            _userRepoMock.Verify(
                r => r.UpdateAsync(userToUpdate, It.IsAny<CancellationToken>()),
                Times.Once);
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

        #region Verify Moq Usage Tests

        [Test]
        public void Setup_ShouldNeverCallRepository_OnTestSetup()
        {
            // Assert - repository by neměl být volán
            _userRepoMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetByIdAsync_ShouldCallRepositoryExactlyOnce()
        {
            // Arrange
            var userId = 1;
            _userRepoMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            // Act
            await _userService.GetByIdAsync(userId);

            // Assert
            _userRepoMock.Verify(
                r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()),
                Times.Once);

            _userRepoMock.VerifyNoOtherCalls();
        }

        #endregion
    }
}
