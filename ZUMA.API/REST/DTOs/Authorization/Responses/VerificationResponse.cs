using ZUMA.API.REST.DTOs.User;

namespace ZUMA.API.REST.DTOs.Authorization.Responses;

public class VerificationResponse
{
    public UserDto? User { get; set; }
    public string Token { get; set; }
}
