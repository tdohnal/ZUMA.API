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
            var ControlsElement = await _controlsElementService.GetByPublicIdAsync(msg.PublicId);

            if (ControlsElement == null)
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
                    PublicId = ControlsElement.PublicId,
                    ControlsElementName = ControlsElement.ControlsElementName,
                    Name = ControlsElement.FullName,
                    Email = ControlsElement.Email,
                    Created = ControlsElement.Created,
                    Updated = ControlsElement.Updated,
                    Deleted = ControlsElement.Deleted
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