using MassTransit;
using Microsoft.Extensions.Logging;
using Zuma.Customer.Domain.Entities;
using Zuma.Customer.Domain.Interfaces;
using ZUMA.SharedKernel.Messagges.Contracts.Users;

namespace ZUMA.CustomerService.Consumers.Users;

public class CreateUserConsumer : IConsumer<SendCreateUserRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<CreateUserConsumer> _logger;

    public CreateUserConsumer(
        IUserService userService,
        ILogger<CreateUserConsumer> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendCreateUserRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Creating user: {Username}", msg.Username);

        try
        {
            var userEntity = new UserEntity
            {
                UserName = msg.Username,
                FullName = msg.FullName,
                Email = msg.Email
            };

            var user = await _userService.CreateAsync(userEntity);

            if (user == null)
            {
                await context.RespondAsync<SendUserFailed>(new
                {
                    ErrorMessage = "Failed to create user.",
                    ErrorCode = "CREATE_FAILED"
                });
                return;
            }

            await context.RespondAsync<SendCreateUserSuccess>(new SendCreateUserSuccess
            {
                User = new UserMessageModel
                {
                    PublicId = user.PublicId,
                    UserName = user.UserName,
                    Name = user.FullName,
                    Email = user.Email,
                    Created = user.Created,
                    Updated = user.Updated,
                    Deleted = user.Deleted
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user: {Username}", msg.Username);

            await context.RespondAsync<SendUserFailed>(new
            {
                ErrorMessage = "An internal error occurred while creating the user.",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }
}
