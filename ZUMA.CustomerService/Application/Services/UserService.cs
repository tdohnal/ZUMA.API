using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Services;
using ZUMA.SharedKernel.Utils;

namespace ZUMA.CustomerService.Application.Services;

internal class UserService : ServiceBase<UserEntity>, IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IEventPublisherService _eventPublisherService;
    private readonly IConfiguration _config;

    public UserService(
        IUserRepository userRepository,
        IEventPublisherService eventPublisherService,
        ILogger<UserService> logger,
        IConfiguration config
        ) : base(userRepository)
    {
        _userRepository = userRepository;
        _eventPublisherService = eventPublisherService;
        _logger = logger;
        _config = config;
    }

    protected override Task BeforeCreateAsync(UserEntity entity, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new user with username: {Username}", entity.UserName);
        return base.BeforeCreateAsync(entity, cancellationToken);
    }

    public async Task<long?> GetIdByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentNullException(nameof(email));
        }

        try
        {
            var ret = await _userRepository.GetByEmailAsync(email, cancellationToken);
            return ret?.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ID by email {Email}", email);
            throw;
        }
    }

    public async Task<VerificationResult> VerificateAuthorizationCode(string code, string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentNullException(nameof(code));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));

        try
        {
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null) return new VerificationResult("User not found");

            if (user.AuthCode != code) return new VerificationResult("Invalid authorization code");

            if (user.AuthCodeExpiration < DateTime.UtcNow) return new VerificationResult("Authorization code expired");

            var token = CreateJwtToken(user);

            user.AuthCode = null;
            user.AuthCodeExpiration = null;

            await _userRepository.UpdateAsync(user, cancellationToken);

            return new VerificationResult { User = user, Token = token };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Verification failed for email {Email}", email);
            throw;
        }

        throw new NotImplementedException();
    }

    public async Task GetAuthorizationCodeAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null) return;

            var code = CodeGenerator.GenerateNumericCode(6);
            user.AuthCode = code;
            user.AuthCodeExpiration = DateTime.UtcNow.AddMinutes(15);

            await _userRepository.UpdateAsync(user, cancellationToken);

            await _eventPublisherService.PublishCreateEmailEventAsync(
                 new SharedKernel.Messagges.Events.CreateEmailEvent
                 {
                     UserId = user.PublicId,
                     Email = user.Email,
                     Code = code,
                     Subject = $"Váš ověřovací kód ZUMA [{code}]",
                     FullName = user.FullName,
                     EmailTemplateType = EmailTemplateType.Authorization,
                 }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating auth code for user ID {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Privátní metoda pro vytvoření JWT řetězce
    /// </summary>
    private string CreateJwtToken(UserEntity user)
    {
        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow, // Tímto tam přidáš claim 'nbf'
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}