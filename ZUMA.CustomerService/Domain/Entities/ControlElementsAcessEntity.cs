using System.ComponentModel.DataAnnotations;
using ZUMA.SharedKernel.Entities;

namespace ZUMA.CustomerService.Domain.Entities;

public class ControlElementsAcessEntity : IAuditableEntities
{
    [Key]
    public long Id { get; set; }
    public Guid PublicId { get; set; } = Guid.CreateVersion7();
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Deleted { get; set; }


    public long UserControlElementId { get; set; }
    public long UserId { get; set; }
    public long CanView { get; set; }
    public long CanEdit { get; set; }
    public long CanDelete { get; set; }

}
