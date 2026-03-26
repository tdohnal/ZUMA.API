using ZUMA.BussinessLogic.Entities.Customer;

namespace ZUMA.API.REST.DTOs.User;

[GenerateMapping(typeof(UserEntity))]
public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Deleted { get; set; }
    public bool IsConfirmed { get; set; }
}
