using MassTransit;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.ControlsElement;

namespace ZUMA.CustomerService.Application.Consumers.ControlsElement;

public class DeleteControlsElementConsumer : IConsumer<SendDeleteControlsElementRequest>
{
    private readonly IControlsElementService _controlsElementService;
    private readonly ILogger<DeleteControlsElementConsumer> _logger;

    public DeleteControlsElementConsumer(
        IControlsElementService controlsElementService,
        ILogger<DeleteControlsElementConsumer> logger)
    {
        _controlsElementService = controlsElementService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendDeleteControlsElementRequest> context)
    {
        SendDeleteControlsElementRequest msg = context.Message;
        _logger.LogInformation("Deleting ControlsElement: {PublicId}", msg.PublicId);

        try
        {
            ControlsElementEntity? existingControlsElement = await _controlsElementService.GetByPublicIdAsync(msg.PublicId);

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

            bool success = await _controlsElementService.DeleteAsync(existingControlsElement.Id);

            if (!success)
            {
                await context.RespondAsync<SendControlsElementFailed>(new
                {
                    ErrorMessage = "Failed to delete ControlsElement.",
                    ErrorCode = "DELETE_FAILED"
                });
                return;
            }

            await context.RespondAsync(new SendDeleteControlsElementSuccess());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting ControlsElement: {PublicId}", msg.PublicId);

            await context.RespondAsync<SendControlsElementFailed>(new
            {
                ErrorMessage = "An internal error occurred while deleting the ControlsElement.",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }
}
