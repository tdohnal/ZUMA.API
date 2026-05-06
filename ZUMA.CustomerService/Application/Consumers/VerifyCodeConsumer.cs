using MassTransit;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Authorization;

namespace ZUMA.CustomerService.Application.Consumers;

public class VerifyCodeConsumer : BaseConsumer<SendVerifyCodeRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<VerifyCodeConsumer> _logger;

    public VerifyCodeConsumer(
        IUserService userService,
        ILogger<VerifyCodeConsumer> logger) : base(logger)
    {
        _userService = userService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<SendVerifyCodeRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Processing registration request for email: {Email}", msg.Email);

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

            await context.RespondAsync(new VerificationSuccess
            {
                User = userMessage,
                Token = ret.Token
            });
        }
    }

    protected override Task OnFailedAsync<TFailedResponse>(ConsumeContext<SendVerifyCodeRequest> context, Exception ex)
    {
        return base.OnFailedAsync<VerificationFailed>(context, ex);
    }
}