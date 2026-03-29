using ZUMA.BussinessLogic.Services;
using ZUMA.BussinessLogic.Utils;
using ZUMA.CustomerService.Entities;
using ZUMA.CustomerService.Repositories.Registration;
using ZUMA.CustomerService.Services.User;

namespace ZUMA.CustomerService.Services.Registration;

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

        var user = new UserEntity
        {
            Email = entity.User.Email,
            FullName = entity.User.FullName,
            UserName = entity.User.Email,
        };
        _userService.CreateAsync(user, cancellationToken).Wait();

        entity.UserId = entity.User.Id;
        entity.User = user;

        _logger.LogInformation("User created with Id:{id} for registration", entity.Id);

        return base.BeforeCreateAsync(entity, cancellationToken);
    }

    protected override Task AfterCreateAsync(RegistrationEntity entity, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registration created with Id:{id} for user with email: {email}", entity.Id, entity.User.Email);
        //_emailService.CreateAsync(new EmailEntity
        //{
        //    Recipient = entity.User,
        //    RecipientId = entity.Id,
        //    Subject = "Your Registration was successed",
        //    Body = $"Welcome in ZUMA team!",
        //    EmailTemplateType = EmailTemplateType.RegistrationVerify

        //}, cancellationToken).Wait();

        return base.AfterCreateAsync(entity, cancellationToken);
    }
}
