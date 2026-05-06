using MassTransit;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Users;

namespace ZUMA.CustomerService.Application.Consumers.Users;

public class GetUsersConsumer : BaseConsumer<SendGetUsersRequest>
{
    private readonly IUserService _userService;
    private readonly ILogger<GetUsersConsumer> _logger;

    public GetUsersConsumer(
        IUserService userService,
        ILogger<GetUsersConsumer> logger) : base(logger)
    {
        _userService = userService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<SendGetUsersRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Fetching all users data");

        IList<UserEntity> users = await _userService.GetAllAsync();

        if (users == null)
        {
            throw new Exception($"{nameof(users)} IS NULL");
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

        await context.RespondAsync(new SendGetUsersSuccess
        {
            User = data
        });
    }

    protected override Task OnFailedAsync<TFailedResponse>(ConsumeContext<SendGetUsersRequest> context, Exception ex)
    {
        return base.OnFailedAsync<SendUserFailed>(context, ex);
    }
}