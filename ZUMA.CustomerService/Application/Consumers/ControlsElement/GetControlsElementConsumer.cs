using MassTransit;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.ControlsElement;

namespace ZUMA.CustomerService.Application.Consumers.ControlsElement;

public class GetControlsElementConsumer : IConsumer<SendGetControlsElementsRequest>
{
    private readonly IControlsElementService _controlsElementService;
    private readonly ILogger<GetControlsElementConsumer> _logger;

    public GetControlsElementConsumer(
        IControlsElementService controlsElementService,
        ILogger<GetControlsElementConsumer> logger)
    {
        _controlsElementService = controlsElementService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendGetControlsElementsRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Fetching all ControlsElements data");

        try
        {
            IList<ControlsElementEntity> controlsElements = await _controlsElementService.GetAllAsync();

            if (controlsElements == null)
            {
                await context.RespondAsync<SendControlsElementFailed>(new
                {
                    ErrorMessage = $"ControlsElements IS NULL",
                    ErrorCode = "ControlsElementS_NOT_FOUND"
                });
                return;
            }

            var data = controlsElements.Select(controlsElement => new ControlsElementMessageModel
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
                    PublicId = x.PublicId,
                    Metadata = x.Metadata,
                    Deleted = x.Deleted
                }).ToList(),
                OwnerUserPublicId = controlsElement.OwnerUser.PublicId,
                PublicId = controlsElement.PublicId,
                ElementsPermission = controlsElement.ElementsPermission
            }).ToList();

            await context.RespondAsync<SendGetControlsElementsSuccess>(new SendGetControlsElementsSuccess
            {
                ControlsElement = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching ControlsElements");

            await context.RespondAsync<SendControlsElementFailed>(new
            {
                ErrorMessage = "An internal error occurred while retrieving ControlsElement data.",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }
}