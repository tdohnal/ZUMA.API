using MassTransit;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.ControlsElement;

namespace ZUMA.CustomerService.Application.Consumers.ControlsElement;

public class UpdateControlsElementConsumer : BaseConsumer<SendUpdateControlsElementRequest>
{
    private readonly IControlsElementService _controlsElementService;
    private readonly ILogger<UpdateControlsElementConsumer> _logger;

    public UpdateControlsElementConsumer(
        IControlsElementService controlsElementService,
        ILogger<UpdateControlsElementConsumer> logger) : base(logger)
    {
        _controlsElementService = controlsElementService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<SendUpdateControlsElementRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Updating ControlsElement: {PublicId}", msg.PublicId);

        var existingControlsElement = await _controlsElementService.GetByPublicIdAsync(msg.PublicId);

        if (existingControlsElement == null)
        {
            _logger.LogWarning("ControlsElement with PublicId: {PublicId} not found.", msg.PublicId);

            await context.RespondAsync<SendControlsElementFailed>(new
            {
                ErrorMessage = $"ControlsElement with ID {msg.PublicId} was not found.",
                ErrorCode = "ControlsElement_NOT_FOUND"
            });
            return;
        }


        existingControlsElement.Title = msg.Title;
        existingControlsElement.Items = msg.Items.Select(x => new Domain.Entities.ControlsElementsItemEntity
        {
            Content = x.Content,
            ControlElementId = existingControlsElement.Id,
            Metadata = x.Metadata,
            PublicId = x.PublicId,
            Created = DateTime.UtcNow,

        }).ToList();
        existingControlsElement.ElementsPermission = msg.ElementsPermission;

        var controlsElement = await _controlsElementService.UpdateAsync(existingControlsElement);

        if (controlsElement == null)
        {
            await context.RespondAsync<SendControlsElementFailed>(new
            {
                ErrorMessage = "Failed to update ControlsElement.",
                ErrorCode = "UPDATE_FAILED"
            });
            return;
        }

        await context.RespondAsync(new SendUpdateControlsElementSuccess
        {
            ControlsElement = new ControlsElementMessageModel
            {
                Title = controlsElement.Title,
                Created = controlsElement.Created,
                Updated = controlsElement.Updated,
                Deleted = controlsElement.Deleted,
                ListType = controlsElement.ListType,
                Items = controlsElement.Items.Select(x => new ControlsElementsItemModel
                {
                    Content = x.Content,
                    ControlElementPublicId = controlsElement.PublicId,
                    Created = x.Created,
                    Updated = controlsElement.Updated,
                    PublicId = controlsElement.PublicId,
                    Metadata = x.Metadata,
                    Deleted = x.Deleted
                }).ToList(),
                OwnerUserPublicId = controlsElement.OwnerUser.PublicId,
                PublicId = controlsElement.PublicId,
                ElementsPermission = controlsElement.ElementsPermission
            }
        });
    }

    protected override Task OnFailedAsync<TFailedResponse>(ConsumeContext<SendUpdateControlsElementRequest> context, Exception ex)
    {
        return base.OnFailedAsync<SendControlsElementFailed>(context, ex);
    }
}
