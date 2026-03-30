using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using ZUMA.BussinessLogic.Messagges.Contracts.Users;
using ZUMA.CustomerService.Consumers.Users;
using ZUMA.CustomerService.Entities;
using ZUMA.CustomerService.Services.User;

namespace ZUMA.Unittests
{
    [TestFixture]
    public class UserConsumerTests
    {
        private Mock<IUserService> _userServiceMock = null!;
        private Mock<ILogger<CreateUserConsumer>> _createLoggerMock = null!;
        private Mock<ILogger<UpdateUserConsumer>> _updateLoggerMock = null!;
        private Mock<ILogger<DeleteUserConsumer>> _deleteLoggerMock = null!;
        private Mock<ConsumeContext<SendCreateUserRequest>> _createContextMock = null!;
        private Mock<ConsumeContext<SendUpdateUserRequest>> _updateContextMock = null!;
        private Mock<ConsumeContext<SendDeleteUserRequest>> _deleteContextMock = null!;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _createLoggerMock = new Mock<ILogger<CreateUserConsumer>>();
            _updateLoggerMock = new Mock<ILogger<UpdateUserConsumer>>();
            _deleteLoggerMock = new Mock<ILogger<DeleteUserConsumer>>();
            _createContextMock = new Mock<ConsumeContext<SendCreateUserRequest>>();
            _updateContextMock = new Mock<ConsumeContext<SendUpdateUserRequest>>();
            _deleteContextMock = new Mock<ConsumeContext<SendDeleteUserRequest>>();
        }

        #region CreateUserConsumer Tests

        [Test]
        public async Task CreateUserConsumer_ShouldCreateUser_WhenValidRequestProvided()
        {
            // Arrange
            var request = new SendCreateUserRequest
            {
                Username = "newuser",
                FullName = "New User",
                Email = "newuser@example.com"
            };

            var createdUser = new UserEntity
            {
                Id = 1,
                PublicId = Guid.NewGuid(),
                UserName = "newuser",
                FullName = "New User",
                Email = "newuser@example.com",
                Created = DateTime.UtcNow
            };

            _createContextMock.Setup(x => x.Message).Returns(request);
            _userServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdUser);

            var consumer = new CreateUserConsumer(_userServiceMock.Object, _createLoggerMock.Object);

            // Act
            await consumer.Consume(_createContextMock.Object);

            // Assert
            _createContextMock.Verify(
                x => x.RespondAsync<SendCreateUserSuccess>(It.IsAny<object>(), It.IsAny<IPipe<SendContext<SendCreateUserSuccess>>>()),
                Times.Once);
            _userServiceMock.Verify(
                x => x.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task CreateUserConsumer_ShouldRespondWithFailure_WhenCreationReturnsNull()
        {
            // Arrange
            var request = new SendCreateUserRequest
            {
                Username = "newuser",
                FullName = "New User",
                Email = "newuser@example.com"
            };

            _createContextMock.Setup(x => x.Message).Returns(request);
            _userServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            var consumer = new CreateUserConsumer(_userServiceMock.Object, _createLoggerMock.Object);

            // Act
            await consumer.Consume(_createContextMock.Object);

            // Assert
            _createContextMock.Verify(
                x => x.RespondAsync<SendUserFailed>(It.IsAny<object>(), It.IsAny<IPipe<SendContext<SendUserFailed>>>()),
                Times.Once);
        }

        [Test]
        public async Task CreateUserConsumer_ShouldRespondWithFailure_WhenExceptionOccurs()
        {
            // Arrange
            var request = new SendCreateUserRequest
            {
                Username = "newuser",
                FullName = "New User",
                Email = "newuser@example.com"
            };

            _createContextMock.Setup(x => x.Message).Returns(request);
            _userServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var consumer = new CreateUserConsumer(_userServiceMock.Object, _createLoggerMock.Object);

            // Act
            await consumer.Consume(_createContextMock.Object);

            // Assert
            _createContextMock.Verify(
                x => x.RespondAsync<SendUserFailed>(It.IsAny<object>(), It.IsAny<IPipe<SendContext<SendUserFailed>>>()),
                Times.Once);
        }

        #endregion

        #region UpdateUserConsumer Tests

        [Test]
        public async Task UpdateUserConsumer_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange
            var publicId = Guid.NewGuid();
            var request = new SendUpdateUserRequest
            {
                PublicId = publicId,
                Username = "updateduser",
                FullName = "Updated User",
                Email = "updated@example.com"
            };

            var existingUser = new UserEntity
            {
                Id = 1,
                PublicId = publicId,
                UserName = "olduser",
                FullName = "Old User",
                Email = "old@example.com",
                Created = DateTime.UtcNow
            };

            var updatedUser = new UserEntity
            {
                Id = 1,
                PublicId = publicId,
                UserName = "updateduser",
                FullName = "Updated User",
                Email = "updated@example.com",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            _updateContextMock.Setup(x => x.Message).Returns(request);
            _userServiceMock
                .Setup(x => x.GetByPublicIdAsync(publicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);
            _userServiceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedUser);

            var consumer = new UpdateUserConsumer(_userServiceMock.Object, _updateLoggerMock.Object);

            // Act
            await consumer.Consume(_updateContextMock.Object);

            // Assert
            _updateContextMock.Verify(
                x => x.RespondAsync<SendUpdateUserSuccess>(It.IsAny<object>(), It.IsAny<IPipe<SendContext<SendUpdateUserSuccess>>>()),
                Times.Once);
            _userServiceMock.Verify(
                x => x.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task UpdateUserConsumer_ShouldRespondWithFailure_WhenUserNotFound()
        {
            // Arrange
            var publicId = Guid.NewGuid();
            var request = new SendUpdateUserRequest
            {
                PublicId = publicId,
                Username = "updateduser",
                FullName = "Updated User",
                Email = "updated@example.com"
            };

            _updateContextMock.Setup(x => x.Message).Returns(request);
            _userServiceMock
                .Setup(x => x.GetByPublicIdAsync(publicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            var consumer = new UpdateUserConsumer(_userServiceMock.Object, _updateLoggerMock.Object);

            // Act
            await consumer.Consume(_updateContextMock.Object);

            // Assert
            _updateContextMock.Verify(
                x => x.RespondAsync<SendUserFailed>(It.IsAny<object>(), It.IsAny<IPipe<SendContext<SendUserFailed>>>()),
                Times.Once);
        }

        [Test]
        public async Task UpdateUserConsumer_ShouldRespondWithFailure_WhenUpdateReturnsNull()
        {
            // Arrange
            var publicId = Guid.NewGuid();
            var request = new SendUpdateUserRequest
            {
                PublicId = publicId,
                Username = "updateduser",
                FullName = "Updated User",
                Email = "updated@example.com"
            };

            var existingUser = new UserEntity
            {
                Id = 1,
                PublicId = publicId,
                UserName = "olduser"
            };

            _updateContextMock.Setup(x => x.Message).Returns(request);
            _userServiceMock
                .Setup(x => x.GetByPublicIdAsync(publicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);
            _userServiceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            var consumer = new UpdateUserConsumer(_userServiceMock.Object, _updateLoggerMock.Object);

            // Act
            await consumer.Consume(_updateContextMock.Object);

            // Assert
            _updateContextMock.Verify(
                x => x.RespondAsync<SendUserFailed>(It.IsAny<object>(), It.IsAny<IPipe<SendContext<SendUserFailed>>>()),
                Times.Once);
        }

        #endregion

        #region DeleteUserConsumer Tests

        [Test]
        public async Task DeleteUserConsumer_ShouldDeleteUser_WhenUserExists()
        {
            // Arrange
            var publicId = Guid.NewGuid();
            var request = new SendDeleteUserRequest
            {
                PublicId = publicId
            };

            var userToDelete = new UserEntity
            {
                Id = 1,
                PublicId = publicId,
                UserName = "usertodeleted",
                FullName = "User To Delete",
                Email = "delete@example.com",
                Created = DateTime.UtcNow
            };

            _deleteContextMock.Setup(x => x.Message).Returns(request);
            _userServiceMock
                .Setup(x => x.GetByPublicIdAsync(publicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userToDelete);
            _userServiceMock
                .Setup(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var consumer = new DeleteUserConsumer(_userServiceMock.Object, _deleteLoggerMock.Object);

            // Act
            await consumer.Consume(_deleteContextMock.Object);

            // Assert
            _deleteContextMock.Verify(
                x => x.RespondAsync<SendDeleteUserSuccess>(It.IsAny<object>(), It.IsAny<IPipe<SendContext<SendDeleteUserSuccess>>>()),
                Times.Once);
            _userServiceMock.Verify(
                x => x.DeleteAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task DeleteUserConsumer_ShouldRespondWithFailure_WhenUserNotFound()
        {
            // Arrange
            var publicId = Guid.NewGuid();
            var request = new SendDeleteUserRequest
            {
                PublicId = publicId
            };

            _deleteContextMock.Setup(x => x.Message).Returns(request);
            _userServiceMock
                .Setup(x => x.GetByPublicIdAsync(publicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            var consumer = new DeleteUserConsumer(_userServiceMock.Object, _deleteLoggerMock.Object);

            // Act
            await consumer.Consume(_deleteContextMock.Object);

            // Assert
            _deleteContextMock.Verify(
                x => x.RespondAsync<SendUserFailed>(It.IsAny<object>(), It.IsAny<IPipe<SendContext<SendUserFailed>>>()),
                Times.Once);
        }

        [Test]
        public async Task DeleteUserConsumer_ShouldRespondWithFailure_WhenDeleteFails()
        {
            // Arrange
            var publicId = Guid.NewGuid();
            var request = new SendDeleteUserRequest
            {
                PublicId = publicId
            };

            var userToDelete = new UserEntity
            {
                Id = 1,
                PublicId = publicId,
                UserName = "usertodeleted"
            };

            _deleteContextMock.Setup(x => x.Message).Returns(request);
            _userServiceMock
                .Setup(x => x.GetByPublicIdAsync(publicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userToDelete);
            _userServiceMock
                .Setup(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var consumer = new DeleteUserConsumer(_userServiceMock.Object, _deleteLoggerMock.Object);

            // Act
            await consumer.Consume(_deleteContextMock.Object);

            // Assert
            _deleteContextMock.Verify(
                x => x.RespondAsync<SendUserFailed>(It.IsAny<object>(), It.IsAny<IPipe<SendContext<SendUserFailed>>>()),
                Times.Once);
        }

        [Test]
        public async Task DeleteUserConsumer_ShouldRespondWithFailure_WhenExceptionOccurs()
        {
            // Arrange
            var publicId = Guid.NewGuid();
            var request = new SendDeleteUserRequest
            {
                PublicId = publicId
            };

            _deleteContextMock.Setup(x => x.Message).Returns(request);
            _userServiceMock
                .Setup(x => x.GetByPublicIdAsync(publicId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var consumer = new DeleteUserConsumer(_userServiceMock.Object, _deleteLoggerMock.Object);

            // Act
            await consumer.Consume(_deleteContextMock.Object);

            // Assert
            _deleteContextMock.Verify(
                x => x.RespondAsync<SendUserFailed>(It.IsAny<object>(), It.IsAny<IPipe<SendContext<SendUserFailed>>>()),
                Times.Once);
        }

        #endregion
    }
}
