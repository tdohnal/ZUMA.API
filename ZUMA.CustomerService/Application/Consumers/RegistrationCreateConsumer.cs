using MassTransit;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Authorization;

namespace ZUMA.CustomerService.Application.Consumers;

public class RegistrationCreateConsumer : BaseConsumer<SendRegistrationCreateRequest>
{
    private readonly IRegistrationService _registrationService;
    private readonly ILogger<RegistrationCreateConsumer> _logger;

    public RegistrationCreateConsumer(
        IRegistrationService registrationService,
        ILogger<RegistrationCreateConsumer> logger) : base(logger)
    {
        _registrationService = registrationService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<SendRegistrationCreateRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Processing registration request for email: {Email}", msg.Email);

        RegistrationEntity newReg = new()
        {
            Created = DateTime.UtcNow,
            User = new UserEntity
            {
                FullName = $"{msg.FirstName} {msg.LastName}",
                Email = msg.Email,
                UserName = msg.Username
            }
        };

        await _registrationService.CreateAsync(newReg);

        _logger.LogInformation("Registration successfully created. PublicId: {PublicId}", newReg.PublicId);

        await context.RespondAsync<RegistrateSuccess>(new
        {
            PublicId = newReg.PublicId,
            Message = "Registration record created successfully."
        });
    }

    protected override Task OnFailedAsync<TFailedResponse>(ConsumeContext<SendRegistrationCreateRequest> context, Exception ex)
    {
        return base.OnFailedAsync<RegistrateFailed>(context, ex);
    }

}
