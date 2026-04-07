using MassTransit;
using Microsoft.Extensions.Logging;
using Zuma.Customer.Domain.Interfaces;
using ZUMA.SharedKernel.Messagges.Contracts.Authorization;

namespace ZUMA.CustomerService.Consumers;

public class AuthorizeUserConsumer : IConsumer<SendAuthorizeUserRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthorizeUserConsumer> _logger;

    public AuthorizeUserConsumer(
        IUserService userService,
        ILogger<AuthorizeUserConsumer> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendAuthorizeUserRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Processing registration request for email: {Email}", msg.Email);

        try
        {
            var id = await _userService.GetIdByEmailAsync(msg.Email);

            if (id == null)
            {
                await context.RespondAsync<AuthorizeUserFailed>(new
                {
                    ErrorMessage = $"Not found by {msg.Email}"
                });
                return;
            }

            if (id.HasValue)
            {
                await _userService.GetAuthorizationCodeAsync(id.Value);
            }

            await context.RespondAsync<AuthorizeUserSuccess>(new
            {
                Message = "Authorization record created successfully."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create authorize for email: {Email}", msg.Email);

            await context.RespondAsync<AuthorizeUserFailed>(new
            {
                ErrorMessage = "Internal database error during authorization user process.",
                ErrorCode = "DB_SAVE_ERROR"
            });
        }
    }
}