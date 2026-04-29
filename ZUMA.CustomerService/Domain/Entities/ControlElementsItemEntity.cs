using System.ComponentModel.DataAnnotations;
using ZUMA.SharedKernel.Domain.Interfaces;

namespace ZUMA.CustomerService.Domain.Entities;

public class ControlsElementsItemEntity : IAuditableEntities
{
    [Key]
    public long Id { get; set; }
    public Guid PublicId { get; set; } = Guid.CreateVersion7();
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Deleted { get; set; }
    public long ControlElementId { get; set; }
    public ControlsElementEntity? ControlElement { get; set; }
    public required string Content { get; set; }
    public string? Metadata { get; set; }
}
