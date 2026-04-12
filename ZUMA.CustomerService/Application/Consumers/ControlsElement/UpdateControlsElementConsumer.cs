using MassTransit;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.MessagingContracts.Contracts.ControlsElement;

namespace ZUMA.CustomerService.Application.Consumers.ControlsElement;

public class UpdateControlsElementConsumer : IConsumer<SendUpdateControlsElementRequest>
{
    private readonly IControlsElementService _controlsElementService;
    private readonly ILogger<UpdateControlsElementConsumer> _logger;

    public UpdateControlsElementConsumer(
        IControlsElementService controlsElementService,
        ILogger<UpdateControlsElementConsumer> logger)
    {
        _controlsElementService = controlsElementService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendUpdateControlsElementRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Updating ControlsElement: {PublicId}", msg.PublicId);

        try
        {
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

            // Update fields
            existingControlsElement.ControlsElementName = msg.ControlsElementname;
            existingControlsElement.FullName = msg.FullName;
            existingControlsElement.Email = msg.Email;

            var ControlsElement = await _controlsElementService.UpdateAsync(existingControlsElement);

            if (ControlsElement == null)
            {
                await context.RespondAsync<SendControlsElementFailed>(new
                {
                    ErrorMessage = "Failed to update ControlsElement.",
                    ErrorCode = "UPDATE_FAILED"
                });
                return;
            }

            await context.RespondAsync<SendUpdateControlsElementSuccess>(new SendUpdateControlsElementSuccess
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
            _logger.LogError(ex, "Error occurred while updating ControlsElement: {PublicId}", msg.PublicId);

            await context.RespondAsync<SendControlsElementFailed>(new
            {
                ErrorMessage = "An internal error occurred while updating the ControlsElement.",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }
}
