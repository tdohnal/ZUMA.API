using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Application.Services;
using ZUMA.SharedKernel.Application.Utils;

namespace ZUMA.CustomerService.Application.Services;

internal class RegistrationService : ServiceBase<RegistrationEntity>, IRegistrationService
{
    private readonly ILogger<RegistrationService> _logger;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IEventPublisherService _eventPublisherService;
    private readonly IUserService _userService;

    public RegistrationService
        (
        IRegistrationRepository registrationRepository,
        IEventPublisherService eventPublisherService,
        IUserService userService,
        ILogger<RegistrationService> logger
        ) : base(registrationRepository)
    {
        _logger = logger;
        _userService = userService;
        _registrationRepository = registrationRepository;
        _eventPublisherService = eventPublisherService;
    }

    protected override Task BeforeCreateAsync(RegistrationEntity entity, CancellationToken cancellationToken)
    {
        entity.ActivationCode = PasswordGenerator.Generate(10);
        entity.ExpirationCodeDate = DateTime.UtcNow.AddHours(24);

        UserEntity user = new()
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

    protected override async Task AfterCreateAsync(RegistrationEntity entity, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registration created with Id:{id} for user with email: {email}", entity.Id, entity.User.Email);
        await _eventPublisherService.PublishCreateEmailEventAsync(
                new SharedKernel.Domain.MessagingContracts.Events.CreateEmailEvent
                {
                    UserId = entity.User.PublicId,
                    Subject = "Welcome in Zuma",
                    FullName = entity.User.FullName,
                    Email = entity.User.Email,
                    Code = entity.User.AuthCode,
                    EmailTemplateType = EmailTemplateType.RegistrationVerify,
                }, cancellationToken);
    }
}
