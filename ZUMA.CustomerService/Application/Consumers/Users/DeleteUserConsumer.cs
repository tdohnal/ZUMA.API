using MassTransit;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Users;

namespace ZUMA.CustomerService.Application.Consumers.Users;

public class DeleteUserConsumer : BaseConsumer<SendDeleteUserRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<DeleteUserConsumer> _logger;

    public DeleteUserConsumer(
        IUserService userService,
        ILogger<DeleteUserConsumer> logger) : base(logger)
    {
        _userService = userService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<SendDeleteUserRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Deleting user: {PublicId}", msg.PublicId);

        var existingUser = await _userService.GetByPublicIdAsync(msg.PublicId);

        if (existingUser == null)
        {
            _logger.LogWarning("User with PublicId: {PublicId} not found.", msg.PublicId);

            await OnFailedAsync<SendUserFailed>(context, new Exception($"User with ID {msg.PublicId} was not found."));

            return;
        }

        var success = await _userService.DeleteAsync(existingUser.Id);

        await context.RespondAsync<SendDeleteUserSuccess>(new());

    }

    protected override Task OnFailedAsync<TFailedResponse>(ConsumeContext<SendDeleteUserRequest> context, Exception ex)
    {
        return base.OnFailedAsync<SendUserFailed>(context, ex);
    }
}
