using MassTransit;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.MessagingContracts.Contracts.ControlsElement;

namespace ZUMA.CustomerService.Application.Consumers.ControlsElement;

public class GetControlsElementByIdConsumer : IConsumer<SendGetControlsElementByIdRequest>
{
    private readonly IControlsElementService _controlsElementService;
    private readonly ILogger<GetControlsElementByIdConsumer> _logger;

    public GetControlsElementByIdConsumer(
        IControlsElementService controlsElementService,
        ILogger<GetControlsElementByIdConsumer> logger)
    {
        _controlsElementService = controlsElementService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendGetControlsElementByIdRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Fetching ControlsElement data for PublicId: {PublicId}", msg.PublicId);

        try
        {
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

            await context.RespondAsync<SendGetControlsElementByIdSuccess>(new SendGetControlsElementByIdSuccess
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching ControlsElement for PublicId: {PublicId}", msg.PublicId);

            await context.RespondAsync<SendControlsElementFailed>(new
            {
                ErrorMessage = "An internal error occurred while retrieving ControlsElement data.",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }
}