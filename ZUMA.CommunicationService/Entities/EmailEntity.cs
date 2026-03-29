using System.ComponentModel.DataAnnotations.Schema;

namespace ZUMA.BussinessLogic.Entities.Customer;

[Table("Emails")]
public class EmailEntity : IAuditableEntities
{
    public long Id { get; set; }
    public Guid PublicId { get; set; } = Guid.CreateVersion7();
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Deleted { get; set; }

    public Guid RecipientId { get; set; }

    public required string Subject { get; set; }
    public required string Body { get; set; }
    public required EmailTemplateType EmailTemplateType { get; set; }
    public DateTime? Sent { get; set; }
}
