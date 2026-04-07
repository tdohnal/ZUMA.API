using MassTransit;
using Microsoft.Extensions.Logging;
using Zuma.Customer.Domain.Interfaces;
using ZUMA.SharedKernel.Messagges.Contracts.Authorization;

namespace ZUMA.CustomerService.Consumers;

public class VerifyCodeConsumer : IConsumer<SendVerifyCodeRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<VerifyCodeConsumer> _logger;

    public VerifyCodeConsumer(
        IUserService userService,
        ILogger<VerifyCodeConsumer> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendVerifyCodeRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Processing registration request for email: {Email}", msg.Email);

        try
        {
            var ret = await _userService.VerificateAuthorizationCode(msg.Code, msg.Email);

            if (!string.IsNullOrWhiteSpace(ret.ErrorMessage))
            {
                await context.RespondAsync<VerificationFailed>(new
                {
                    ErrorMessage = ret.ErrorMessage,
                });
            }

            if (ret.IsSuccess)
            {
                var userMessage = new VerificationUserMessage
                {
                    PublicId = ret.User.PublicId,
                    UserName = ret.User.UserName,
                    FullName = ret.User.FullName,
                    Email = ret.User.Email,
                    Created = ret.User.Created,
                    Updated = ret.User.Updated,
                    Deleted = ret.User.Deleted
                };

                await context.RespondAsync<VerificationSuccess>(new
                {
                    User = userMessage,
                    Token = ret.Token
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create authorize for verification: {Email}", msg.Email);

            await context.RespondAsync<VerificationFailed>(new
            {
                ErrorMessage = "Internal database error during authorization user process.",
                ErrorCode = "DB_SAVE_ERROR"
            });
        }
    }
}