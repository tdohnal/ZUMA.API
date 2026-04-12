using MassTransit;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.MessagingContracts.Contracts.ControlsElement;

namespace ZUMA.CustomerService.Application.Consumers.ControlsElement;

public class CreateControlsElementConsumer : IConsumer<SendCreateControlsElementRequest>
{
    private readonly IControlsElementService _controlsElementService;
    private readonly IUserService _userService;
    private readonly ILogger<CreateControlsElementConsumer> _logger;

    public CreateControlsElementConsumer(
        IControlsElementService controlsElementService,
        IUserService userService,
        ILogger<CreateControlsElementConsumer> logger)
    {
        _controlsElementService = controlsElementService;
        _userService = userService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendCreateControlsElementRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Creating ControlsElement: {Title}", msg.Title);

        try
        {
            var user = await _userService.GetByPublicIdAsync(msg.OwnerUserPublicId);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var controlsElementEntity = new ControlsElementEntity
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

            var controlsElement = await _controlsElementService.CreateAsync(controlsElementEntity);

            if (controlsElement == null)
            {
                await context.RespondAsync<SendControlsElementFailed>(new
                {
                    ErrorMessage = "Failed to create ControlsElement.",
                    ErrorCode = "CREATE_FAILED"
                });
                return;
            }

            await context.RespondAsync<SendCreateControlsElementSuccess>(new SendCreateControlsElementSuccess
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating ControlsElement: {Title}", msg.Title);

            await context.RespondAsync<SendControlsElementFailed>(new
            {
                ErrorMessage = "An internal error occurred while creating the ControlsElement.",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }
}
