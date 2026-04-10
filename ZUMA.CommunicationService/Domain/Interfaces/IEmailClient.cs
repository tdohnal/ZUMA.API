using ZUMA.CommunicationService.Domain.Entities;

namespace ZUMA.CommunicationService.Domain.Interfaces;

public interface IEmailClient
{
    Task<bool> SendAsync(EmailEntity message, CancellationToken ct = default);
}
