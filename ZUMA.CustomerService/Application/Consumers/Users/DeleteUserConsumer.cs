using MassTransit;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.MessagingContracts.Contracts.Users;

namespace ZUMA.CustomerService.Application.Consumers.Users;

public class DeleteUserConsumer : IConsumer<SendDeleteUserRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<DeleteUserConsumer> _logger;

    public DeleteUserConsumer(
        IUserService userService,
        ILogger<DeleteUserConsumer> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendDeleteUserRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Deleting user: {PublicId}", msg.PublicId);

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

            var success = await _userService.DeleteAsync(existingUser.Id);

            if (!success)
            {
                await context.RespondAsync<SendUserFailed>(new
                {
                    ErrorMessage = "Failed to delete user.",
                    ErrorCode = "DELETE_FAILED"
                });
                return;
            }

            await context.RespondAsync<SendDeleteUserSuccess>(new SendDeleteUserSuccess());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user: {PublicId}", msg.PublicId);

            await context.RespondAsync<SendUserFailed>(new
            {
                ErrorMessage = "An internal error occurred while deleting the user.",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }
}
