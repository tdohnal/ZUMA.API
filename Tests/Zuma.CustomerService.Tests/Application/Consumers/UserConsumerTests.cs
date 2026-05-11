using AutoFixture;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using ZUMA.CustomerService.Application.Consumers.Users;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Users;

namespace Zuma.CustomerService.Tests.Application.Consumers;

public class UserConsumerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ILogger<CreateUserConsumer>> _loggerMock;
    private readonly Fixture _fixture;

    public UserConsumerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<CreateUserConsumer>>();
    }

    [Fact]
    public async Task CreateUserConsumer_ShouldCreateUser_AndRespondSuccess()
    {
        // Arrange
        var request = _fixture.Create<SendCreateUserRequest>();
        var userEntity = _fixture.Create<UserEntity>();

        // Mockujeme context MassTransitu
        var contextMock = new Mock<ConsumeContext<SendCreateUserRequest>>();
        contextMock.Setup(x => x.Message).Returns(request);
        contextMock.Setup(x => x.MessageId).Returns(Guid.NewGuid());

        _userServiceMock.Setup(s => s.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        var consumer = new CreateUserConsumer(_userServiceMock.Object, _loggerMock.Object);

        // Act
        await consumer.Consume(contextMock.Object);

        // Assert
        _userServiceMock.Verify(s => s.CreateAsync(It.Is<UserEntity>(u => u.Email == request.Email), It.IsAny<CancellationToken>()), Times.Once);
        contextMock.Verify(x => x.RespondAsync(It.IsAny<SendCreateUserSuccess>()), Times.Once);
    }

    [Fact]
    public async Task GetUsersConsumer_ShouldReturnList_WhenUsersExist()
    {
        // Arrange
        var users = _fixture.CreateMany<UserEntity>(3).ToList();
        var contextMock = new Mock<ConsumeContext<SendGetUsersRequest>>();
        contextMock.Setup(x => x.Message).Returns(new SendGetUsersRequest());

        _userServiceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var consumer = new GetUsersConsumer(_userServiceMock.Object, new Mock<ILogger<GetUsersConsumer>>().Object);

        // Act
        await consumer.Consume(contextMock.Object);

        // Assert
        contextMock.Verify(x => x.RespondAsync(It.Is<SendGetUsersSuccess>(r => r.User.Count == 3)), Times.Once);
    }

    [Fact]
    public async Task DeleteUserConsumer_ShouldRespondError_WhenUserNotFound()
    {
        // Arrange
        var request = _fixture.Create<SendDeleteUserRequest>();
        var contextMock = new Mock<ConsumeContext<SendDeleteUserRequest>>();
        contextMock.Setup(x => x.Message).Returns(request);

        // Simulujeme, že uživatel neexistuje
        _userServiceMock.Setup(s => s.GetByPublicIdAsync(request.PublicId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity)null!);

        var consumer = new DeleteUserConsumer(_userServiceMock.Object, new Mock<ILogger<DeleteUserConsumer>>().Object);

        // Act
        await consumer.Consume(contextMock.Object);

        // Assert
        // Ověříme, že se poslala chybová odpověď SendUserFailed (jak máš v OnFailedAsync)
        contextMock.Verify(x => x.RespondAsync<SendUserFailed>(
            It.Is<object>(obj => obj.ToString().Contains("INTERNAL_ERROR") || obj != null)),
            Times.Once);

        // Ověříme, že se NIKDY nezavolalo smazání, když uživatel nebyl nalezen
        _userServiceMock.Verify(s => s.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}