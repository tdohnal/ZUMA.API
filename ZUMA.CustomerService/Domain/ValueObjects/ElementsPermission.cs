namespace ZUMA.CustomerService.Domain.ValueObjects;

public class ElementsPermission
{
    public IList<ElementsUserPermission> UserPermissions { get; set; } = [];

}
