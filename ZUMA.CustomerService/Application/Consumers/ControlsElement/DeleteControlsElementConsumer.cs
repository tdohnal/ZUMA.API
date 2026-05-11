using MassTransit;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.ControlsElement;

namespace ZUMA.CustomerService.Application.Consumers.ControlsElement;

public class DeleteControlsElementConsumer : BaseConsumer<SendDeleteControlsElementRequest>
{
    private readonly IControlsElementService _controlsElementService;
    private readonly ILogger<DeleteControlsElementConsumer> _logger;

    public DeleteControlsElementConsumer(
        IControlsElementService controlsElementService,
        ILogger<DeleteControlsElementConsumer> logger) : base(logger)
    {
        _controlsElementService = controlsElementService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<SendDeleteControlsElementRequest> context)
    {
        SendDeleteControlsElementRequest msg = context.Message;
        _logger.LogInformation("Deleting ControlsElement: {PublicId}", msg.PublicId);

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

    protected override Task OnFailedAsync<TFailedResponse>(ConsumeContext<SendDeleteControlsElementRequest> context, Exception ex)
    {
        return base.OnFailedAsync<SendControlsElementFailed>(context, ex);
    }
}
