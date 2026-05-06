using MassTransit;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Users;

namespace ZUMA.CustomerService.Application.Consumers.Users;

public class UpdateUserConsumer : BaseConsumer<SendUpdateUserRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<UpdateUserConsumer> _logger;

    public UpdateUserConsumer(
        IUserService userService,
        ILogger<UpdateUserConsumer> logger) : base(logger)
    {
        _userService = userService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<SendUpdateUserRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Updating user: {PublicId}", msg.PublicId);

        var existingUser = await _userService.GetByPublicIdAsync(msg.PublicId);

        if (existingUser == null)
        {
            _logger.LogWarning("User with PublicId: {PublicId} not found.", msg.PublicId);

            throw new Exception($"User with ID {msg.PublicId} was not found.");
        }

        existingUser.UserName = msg.Username;
        existingUser.FullName = msg.FullName;
        existingUser.Email = msg.Email;

        var user = await _userService.UpdateAsync(existingUser);

        if (user == null)
        {
            throw new Exception("Failed to update user.");
        }

        await context.RespondAsync(new SendUpdateUserSuccess
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

    protected override Task OnFailedAsync<TFailedResponse>(ConsumeContext<SendUpdateUserRequest> context, Exception ex)
    {
        return base.OnFailedAsync<SendUserFailed>(context, ex);
    }
}
