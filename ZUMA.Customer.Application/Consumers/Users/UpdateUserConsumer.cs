using MassTransit;
using Microsoft.Extensions.Logging;
using Zuma.Customer.Domain.Interfaces;
using ZUMA.SharedKernel.Messagges.Contracts.Users;

namespace ZUMA.CustomerService.Consumers.Users;

public class UpdateUserConsumer : IConsumer<SendUpdateUserRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<UpdateUserConsumer> _logger;

    public UpdateUserConsumer(
        IUserService userService,
        ILogger<UpdateUserConsumer> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendUpdateUserRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Updating user: {PublicId}", msg.PublicId);

        try
        {
            var existingUser = await _userService.GetByPublicIdAsync(msg.PublicId);

            if (existingUser == null)
            {
                _logger.LogWarning("User with PublicId: {PublicId} not found.", msg.PublicId);

                await context.RespondAsync<SendUserFailed>(new
                {
                    ErrorMessage = $"User with ID {msg.PublicId} was not found.",
                    ErrorCode = "USER_NOT_FOUND"
                });
                return;
            }

            // Update fields
            existingUser.UserName = msg.Username;
            existingUser.FullName = msg.FullName;
            existingUser.Email = msg.Email;

            var user = await _userService.UpdateAsync(existingUser);

            if (user == null)
            {
                await context.RespondAsync<SendUserFailed>(new
                {
                    ErrorMessage = "Failed to update user.",
                    ErrorCode = "UPDATE_FAILED"
                });
                return;
            }

            await context.RespondAsync<SendUpdateUserSuccess>(new SendUpdateUserSuccess
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
            _logger.LogError(ex, "Error occurred while updating user: {PublicId}", msg.PublicId);

            await context.RespondAsync<SendUserFailed>(new
            {
                ErrorMessage = "An internal error occurred while updating the user.",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }
}
