using MassTransit;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Authorization;

namespace ZUMA.CustomerService.Application.Consumers;

public class AuthorizeUserConsumer : BaseConsumer<SendAuthorizeUserRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthorizeUserConsumer> _logger;

    public AuthorizeUserConsumer(
        IUserService userService,
        ILogger<AuthorizeUserConsumer> logger) : base(logger)
    {
        _userService = userService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<SendAuthorizeUserRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Processing registration request for email: {Email}", msg.Email);

        var id = await _userService.GetIdByEmailAsync(msg.Email);

        if (id == null)
        {
            throw new Exception($"Not found by {msg.Email}");
        }

        if (id.HasValue)
        {
            await _userService.GetAuthorizationCodeAsync(id.Value);
        }

        await context.RespondAsync(new AuthorizeUserSuccess
        {
            SentAt = DateTime.UtcNow
        });
    }

    protected override Task OnFailedAsync<TFailedResponse>(ConsumeContext<SendAuthorizeUserRequest> context, Exception ex)
    {
        return base.OnFailedAsync<AuthorizeUserFailed>(context, ex);
    }
}