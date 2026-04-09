using MassTransit;
using Microsoft.Extensions.Logging;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Messagges.Contracts.Authorization;
using ZUMA.SharedKernel.Messagges.Contracts.Users;

namespace ZUMA.CustomerService.Application.Consumers.Users;

public class GetUsersConsumer : IConsumer<SendGetUsersRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<GetUsersConsumer> _logger;

    public GetUsersConsumer(
        IUserService userService,
        ILogger<GetUsersConsumer> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendGetUsersRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Fetching all users data");

        try
        {
            var users = await _userService.GetAllAsync();

            if (users == null)
            {
                await context.RespondAsync<SendUserFailed>(new
                {
                    ErrorMessage = $"Users IS NULL",
                    ErrorCode = "USERS_NOT_FOUND"
                });
                return;
            }

            var data = users.Select(user => new UserMessageModel
            {
                PublicId = user.PublicId,
                UserName = user.UserName,
                Name = user.FullName,
                Email = user.Email,
                Created = user.Created,
                Updated = user.Updated,
                Deleted = user.Deleted
            }).ToList();

            await context.RespondAsync<SendGetUsersSuccess>(new SendGetUsersSuccess
            {
                User = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching users");

            await context.RespondAsync<AuthorizeUserFailed>(new
            {
                ErrorMessage = "An internal error occurred while retrieving user data.",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }
}