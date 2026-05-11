using MassTransit;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.ControlsElement;

namespace ZUMA.CustomerService.Application.Consumers.ControlsElement;

public class CreateControlsElementConsumer : BaseConsumer<SendCreateControlsElementRequest>
{
    private readonly IControlsElementService _controlsElementService;
    private readonly IUserService _userService;
    private readonly ILogger<CreateControlsElementConsumer> _logger;

    public CreateControlsElementConsumer(
        IControlsElementService controlsElementService,
        IUserService userService,
        ILogger<CreateControlsElementConsumer> logger) : base(logger)
    {
        _controlsElementService = controlsElementService;
        _userService = userService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<SendCreateControlsElementRequest> context)
    {
        SendCreateControlsElementRequest msg = context.Message;
        _logger.LogInformation("Creating ControlsElement: {Title}", msg.Title);

        UserEntity? user = await _userService.GetByPublicIdAsync(msg.OwnerUserPublicId);

        if (user == null)
            throw new NullReferenceException(nameof(user));

        ControlsElementEntity controlsElementEntity = new()
        {
            OwnerUserId = user.Id,
            ListType = msg.ListType,
            Title = msg.Title,
            ElementsPermission = msg.ElementsPermission,
            Items = msg.Items.Select(x => new ControlsElementsItemEntity
            {
                Content = x.Content,
                Created = x.Created,
                Deleted = x.Deleted,
                Metadata = x.Metadata,
                Updated = x.Updated,
            }).ToList()
        };

        ControlsElementEntity? controlsElement = await _controlsElementService.CreateAsync(controlsElementEntity);

        if (controlsElement == null)
        {
            await context.RespondAsync<SendControlsElementFailed>(new
            {
                ErrorMessage = "Failed to create ControlsElement.",
                ErrorCode = "CREATE_FAILED"
            });
            return;
        }

        await context.RespondAsync(new SendCreateControlsElementSuccess
        {
            ControlsElement = new ControlsElementMessageModel
            {
                OwnerUserPublicId = msg.OwnerUserPublicId,
                ListType = controlsElement.ListType,
                PublicId = controlsElement.PublicId,
                Title = controlsElement.Title,
                Created = controlsElement.Created,
                Updated = controlsElement.Updated,
                Deleted = controlsElement.Deleted
            }
        });
    }

    protected override Task OnFailedAsync<TFailedResponse>(ConsumeContext<SendCreateControlsElementRequest> context, Exception ex)
    {
        return base.OnFailedAsync<SendControlsElementFailed>(context, ex);
    }
}
