using MassTransit;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.MessagingContracts.Contracts.ControlsElement;

namespace ZUMA.CustomerService.Application.Consumers.ControlsElement;

public class CreateControlsElementConsumer : IConsumer<SendCreateControlsElementRequest>
{
    private readonly IControlsElementService _controlsElementService;
    private readonly ILogger<CreateControlsElementConsumer> _logger;

    public CreateControlsElementConsumer(
        IControlsElementService controlsElementService,
        ILogger<CreateControlsElementConsumer> logger)
    {
        _controlsElementService = controlsElementService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendCreateControlsElementRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Creating ControlsElement: {ControlsElementname}", msg.ControlsElementname);

        try
        {
            var ControlsElementEntity = new ControlsElementEntity
            {
                ControlsElementName = msg.ControlsElementname,
                FullName = msg.FullName,
                Email = msg.Email
            };

            var ControlsElement = await _controlsElementService.CreateAsync(ControlsElementEntity);

            if (ControlsElement == null)
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
            _logger.LogError(ex, "Error occurred while creating ControlsElement: {ControlsElementname}", msg.ControlsElementname);

            await context.RespondAsync<SendControlsElementFailed>(new
            {
                ErrorMessage = "An internal error occurred while creating the ControlsElement.",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }
}
