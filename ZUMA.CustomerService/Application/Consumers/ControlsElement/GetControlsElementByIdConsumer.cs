using MassTransit;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.ControlsElement;

namespace ZUMA.CustomerService.Application.Consumers.ControlsElement;

public class GetControlsElementByIdConsumer : BaseConsumer<SendGetControlsElementByIdRequest>
{
    private readonly IControlsElementService _controlsElementService;
    private readonly ILogger<GetControlsElementByIdConsumer> _logger;

    public GetControlsElementByIdConsumer(
        IControlsElementService controlsElementService,
        ILogger<GetControlsElementByIdConsumer> logger) : base(logger)
    {
        _controlsElementService = controlsElementService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<SendGetControlsElementByIdRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Fetching ControlsElement data for PublicId: {PublicId}", msg.PublicId);

        var controlsElement = await _controlsElementService.GetByPublicIdAsync(msg.PublicId);

        if (controlsElement == null)
        {
            _logger.LogWarning("ControlsElement with PublicId: {PublicId} not found.", msg.PublicId);

            await context.RespondAsync<SendControlsElementFailed>(new
            {
                ErrorMessage = $"ControlsElement with ID {msg.PublicId} was not found.",
                ErrorCode = "ControlsElement_NOT_FOUND"
            });
            return;
        }

        await context.RespondAsync(new SendGetControlsElementByIdSuccess
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

    protected override Task OnFailedAsync<TFailedResponse>(ConsumeContext<SendGetControlsElementByIdRequest> context, Exception ex)
    {
        return base.OnFailedAsync<SendControlsElementFailed>(context, ex);
    }
}