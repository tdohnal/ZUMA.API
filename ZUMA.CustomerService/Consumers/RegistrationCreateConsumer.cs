using MassTransit;
using ZUMA.BussinessLogic.Messagges.Requests.Authorization.Request.SendRegistrationCreateRequest;
using ZUMA.BussinessLogic.Messagges.Requests.Authorization.Response;
using ZUMA.CustomerService.Entities;
using ZUMA.CustomerService.Services.Registration;

namespace ZUMA.CustomerService.Consumers;

public class RegistrationCreateConsumer : IConsumer<SendRegistrationCreateRequest>
{
    private readonly IRegistrationService _registrationService;
    private readonly ILogger<RegistrationCreateConsumer> _logger;

    public RegistrationCreateConsumer(
        IRegistrationService registrationService,
        ILogger<RegistrationCreateConsumer> logger)
    {
        _registrationService = registrationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendRegistrationCreateRequest> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Processing registration request for email: {Email}", msg.Email);

        try
        {
            var newReg = new RegistrationEntity
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create registration for email: {Email}", msg.Email);

            await context.RespondAsync<RegistrateFailed>(new
            {
                ErrorMessage = "Internal database error during registration process.",
                ErrorCode = "DB_SAVE_ERROR"
            });
        }
    }
}