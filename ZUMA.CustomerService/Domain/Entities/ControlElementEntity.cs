using System.ComponentModel.DataAnnotations;
using ZUMA.SharedKernel.Domain.ValueObjects.Customer.ControlsElement;
using ZUMA.SharedKernel.Entities;
using ZUMA.SharedKernel.Enums;

namespace ZUMA.CustomerService.Domain.Entities;

public class ControlsElementEntity : IAuditableEntities
{
    #region Base

    [Key]
    public long Id { get; set; }
    public Guid PublicId { get; set; } = Guid.CreateVersion7();
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Deleted { get; set; }

    #endregion

    public required string Title { get; set; }

    public required long OwnerUserId { get; set; }
    public UserEntity? OwnerUser { get; set; }

    public required ListType ListType { get; set; }

    public List<ControlsElementsItemEntity> Items { get; set; } = new();
    public ElementsPermission ElementsPermission { get; set; } = new();
}