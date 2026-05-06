using MassTransit;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Users;

namespace ZUMA.CustomerService.Application.Consumers.Users;

public class GetUserByIdConsumer : BaseConsumer<SendGetUserByIdRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<GetUserByIdConsumer> _logger;

    public GetUserByIdConsumer(
        IUserService userService,
        ILogger<GetUserByIdConsumer> logger) : base(logger)
    {
        _userService = userService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<SendGetUserByIdRequest> context)
    {
        var msg = context.Message;

        _logger.LogInformation("Fetching user data for PublicId: {PublicId}", msg.PublicId);


        var user = await _userService.GetByPublicIdAsync(msg.PublicId);

        if (user == null)
        {
            _logger.LogWarning("User with PublicId: {PublicId} not found.", msg.PublicId);

            throw new Exception($"User with ID {msg.PublicId} was not found.");
        }

        await context.RespondAsync(new SendGetUserByIdSuccess
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

    protected override Task OnFailedAsync<TFailedResponse>(ConsumeContext<SendGetUserByIdRequest> context, Exception ex)
    {
        return base.OnFailedAsync<SendUserFailed>(context, ex);
    }
}