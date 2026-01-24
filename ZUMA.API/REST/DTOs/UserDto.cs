using ZUMA.API.REST.Mappers;
using ZUMA.BussinessLogic.Infrastructure.Entities.Customer;

namespace ZUMA.API.REST.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Deleted { get; set; }

    public UserEntity ToEntity()
    {
        return UserMapper.ToEntity(this);
    }
}
