using ZUMA.API.REST.DTOs;
using ZUMA.BussinessLogic.Infrastructure.Entities.Customer;

namespace ZUMA.API.REST.Mappers;

public static class UserMapper
{
    public static UserEntity ToEntity(this UserDto dto)
    {
        return new UserEntity
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            Password = string.Empty,
            UserName = dto.UserName,
            Created = dto.Created,
            Deleted = dto.Deleted,
            Updated = dto.Updated,
        };
    }

    public static UserDto ToDto(this UserEntity entity)
    {
        return new UserDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            UserName = entity.UserName,
            Created = entity.Created,
            Deleted = entity.Deleted,
            Updated = entity.Updated ?? DateTime.MinValue,
        };
    }
}
