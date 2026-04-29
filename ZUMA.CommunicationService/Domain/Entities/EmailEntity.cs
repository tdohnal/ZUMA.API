using System.ComponentModel.DataAnnotations.Schema;
using ZUMA.SharedKernel.Domain.Interfaces;

namespace ZUMA.CommunicationService.Domain.Entities;

[Table("Emails")]
public class EmailEntity : IAuditableEntities
{
    public long Id { get; set; }
    public Guid PublicId { get; set; } = Guid.CreateVersion7();
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Deleted { get; set; }

    public string Recipient { get; set; }

    public required string Subject { get; set; }
    public required string Body { get; set; }
    public required EmailTemplateType EmailTemplateType { get; set; }
    public DateTime? Sent { get; set; }
}
