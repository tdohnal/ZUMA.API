namespace ZUMA.CustomerService.Domain.ValueObjects;

public class ElementsUserPermission
{
    public long UserId { get; set; }
    public long CanView { get; set; }
    public long CanEdit { get; set; }
    public long CanDelete { get; set; }
}
