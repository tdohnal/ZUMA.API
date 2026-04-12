using MassTransit;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.MessagingContracts.Contracts.ControlsElement;

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
            var ControlsElements = await _controlsElementService.GetAllAsync();

            if (ControlsElements == null)
            {
                await context.RespondAsync<SendControlsElementFailed>(new
                {
                    ErrorMessage = $"ControlsElements IS NULL",
                    ErrorCode = "ControlsElementS_NOT_FOUND"
                });
                return;
            }

            var data = ControlsElements.Select(ControlsElement => new ControlsElementMessageModel
            {
                PublicId = ControlsElement.PublicId,
                ControlsElementName = ControlsElement.ControlsElementName,
                Name = ControlsElement.FullName,
                Email = ControlsElement.Email,
                Created = ControlsElement.Created,
                Updated = ControlsElement.Updated,
                Deleted = ControlsElement.Deleted
            }).ToList();

            await context.RespondAsync<SendGetControlsElementsSuccess>(new SendGetControlsElementsSuccess
            {
                ControlsElement = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching ControlsElements");

            await context.RespondAsync<AuthorizeControlsElementFailed>(new
            {
                ErrorMessage = "An internal error occurred while retrieving ControlsElement data.",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }
}