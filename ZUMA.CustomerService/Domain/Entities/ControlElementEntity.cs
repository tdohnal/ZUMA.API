using System.ComponentModel.DataAnnotations;
using ZUMA.CustomerService.Domain.ValueObjects;
using ZUMA.SharedKernel.Entities;

namespace ZUMA.CustomerService.Domain.Entities;

public class ControlElementEntity : IAuditableEntities
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

    public required ListType ListType { get; set; }

    public List<ControlElementsItemEntity> Items { get; set; } = new();
    public ElementsPermission ElementsPermission { get; set; } = new();
}
public enum ListType
{
    ShoppingList = 1,
    Calendar = 2,
    GiftList = 3,
    VoteList = 4,
    CustomList = 5
}