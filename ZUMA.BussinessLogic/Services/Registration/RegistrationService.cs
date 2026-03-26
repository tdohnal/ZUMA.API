using Microsoft.Extensions.Logging;
using ZUMA.BussinessLogic.Entities.Customer;
using ZUMA.BussinessLogic.Repositories.User;
using ZUMA.BussinessLogic.Utils;

namespace ZUMA.BussinessLogic.Services.User;

internal class RegistrationService : ServiceBase<RegistrationEntity>, IRegistrationService
{
    private readonly ILogger<RegistrationService> _logger;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IUserService _userService;

    public RegistrationService
        (
        IRegistrationRepository registrationRepository,
        IUserService userService,
        ILogger<RegistrationService> logger
        ) : base(registrationRepository)
    {
        _logger = logger;
        _userService = userService;
        _registrationRepository = registrationRepository;
    }

    protected override Task BeforeCreateAsync(RegistrationEntity entity, CancellationToken cancellationToken)
    {
        entity.ActivationCode = PasswordGenerator.Generate(10);
        entity.ExpirationCodeDate = DateTime.UtcNow.AddHours(24);

        _logger.LogInformation("User created with Id:{id} for registration", entity.InternalId);

        return base.BeforeCreateAsync(entity, cancellationToken);
    }
}
