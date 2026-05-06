using MassTransit;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Users;

namespace ZUMA.CustomerService.Application.Consumers.Users;

public class CreateUserConsumer : BaseConsumer<SendCreateUserRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<CreateUserConsumer> _logger;

    public CreateUserConsumer(
        IUserService userService,
        ILogger<CreateUserConsumer> logger) : base(logger)
    {
        _userService = userService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<SendCreateUserRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Creating user: {Username}", msg.Username);

        var userEntity = new UserEntity()
        {
            UserName = msg.Username,
            FullName = msg.FullName,
            Email = msg.Email
        };

        UserEntity? user = await _userService.CreateAsync(userEntity);

        if (user == null)
        {
            await OnFailedAsync<SendUserFailed>(context, new Exception($"{nameof(user)} IS NULL!"));
            return;
        }

        await context.RespondAsync(new SendCreateUserSuccess
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

    protected override Task OnFailedAsync<TFailedResponse>(ConsumeContext<SendCreateUserRequest> context, Exception ex)
    {
        return base.OnFailedAsync<SendUserFailed>(context, ex);
    }
}
