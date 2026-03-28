using Microsoft.Extensions.Logging;
using ZUMA.BussinessLogic.Entities.Customer;
using ZUMA.BussinessLogic.Repositories.User;
using ZUMA.BussinessLogic.Services.Email;
using ZUMA.BussinessLogic.Utils;

namespace ZUMA.BussinessLogic.Services.User;

internal class RegistrationService : ServiceBase<RegistrationEntity>, IRegistrationService
{
    private readonly ILogger<RegistrationService> _logger;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;

    public RegistrationService
        (
        IRegistrationRepository registrationRepository,
        IUserService userService,
        IEmailService emailService,
        ILogger<RegistrationService> logger
        ) : base(registrationRepository)
    {
        _logger = logger;
        _userService = userService;
        _registrationRepository = registrationRepository;
        _emailService = emailService;
    }

    protected override Task BeforeCreateAsync(RegistrationEntity entity, CancellationToken cancellationToken)
    {
        entity.ActivationCode = PasswordGenerator.Generate(10);
        entity.ExpirationCodeDate = DateTime.UtcNow.AddHours(24);

        var user = new UserEntity
        {
            Email = entity.User.Email,
            FullName = entity.User.FullName,
            UserName = entity.User.Email,
        };
        _userService.CreateAsync(user, cancellationToken).Wait();

        entity.UserId = entity.User.InternalId;
        entity.User = user;

        _logger.LogInformation("User created with Id:{id} for registration", entity.InternalId);

        return base.BeforeCreateAsync(entity, cancellationToken);
    }

    protected override Task AfterCreateAsync(RegistrationEntity entity, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registration created with Id:{id} for user with email: {email}", entity.InternalId, entity.User.Email);
        _emailService.CreateAsync(new EmailEntity
        {
            Recipient = entity.User,
            RecipientId = entity.InternalId,
            Subject = "Your Registration was successed",
            Body = $"Welcome in ZUMA team!",
            EmailTemplateType = EmailTemplateType.RegistrationVerify

        }, cancellationToken).Wait();

        return base.AfterCreateAsync(entity, cancellationToken);
    }
}
