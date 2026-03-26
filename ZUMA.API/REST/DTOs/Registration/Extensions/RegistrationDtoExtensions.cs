using ZUMA.API.REST.DTOs.Registration.Requests;
using ZUMA.BussinessLogic.Entities.Customer;

namespace ZUMA.API.REST.DTOs.Registration.Extensions;

public static class RegistrationDtoExtensions
{
    public static RegistrationDto ToDto(this RegistrationEntity entity)
    {
        return new RegistrationDto
        {
        };
    }

    public static RegistrationEntity ToEntity(this RegistrationDto dto)
    {
        return new RegistrationEntity
        {
            User = new UserEntity
            {
                Email = dto.Email,
            }
        };
    }

    public static RegistrationEntity ToEntity(this CreateRegistrationRequestDto dto)
    {
        return new RegistrationEntity
        {
            User = new UserEntity
            {
                Email = dto.Email,
                UserName = dto.Username,
                FullName = $"{dto.FirstName} {dto.LastName}",
            },
        };
    }
}
