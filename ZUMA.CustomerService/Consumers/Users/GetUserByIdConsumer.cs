using MassTransit;
using ZUMA.SharedKernel.Messagges.Contracts.Users;
using ZUMA.CustomerService.Services.User;

namespace ZUMA.CustomerService.Consumers.Users;

public class GetUserByIdConsumer : IConsumer<SendGetUserByIdRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<GetUserByIdConsumer> _logger;

    public GetUserByIdConsumer(
        IUserService userService,
        ILogger<GetUserByIdConsumer> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendGetUserByIdRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Fetching user data for PublicId: {PublicId}", msg.PublicId);

        try
        {
            var user = await _userService.GetByPublicIdAsync(msg.PublicId);

            if (user == null)
            {
                _logger.LogWarning("User with PublicId: {PublicId} not found.", msg.PublicId);

                await context.RespondAsync<SendUserFailed>(new
                {
                    ErrorMessage = $"User with ID {msg.PublicId} was not found.",
                    ErrorCode = "USER_NOT_FOUND"
                });
                return;
            }

            await context.RespondAsync<SendGetUserByIdSuccess>(new SendGetUserByIdSuccess
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
            _logger.LogError(ex, "Error occurred while fetching user for PublicId: {PublicId}", msg.PublicId);

            await context.RespondAsync<SendUserFailed>(new
            {
                ErrorMessage = "An internal error occurred while retrieving user data.",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }
}