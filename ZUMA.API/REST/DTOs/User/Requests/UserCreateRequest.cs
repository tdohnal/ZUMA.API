namespace ZUMA.API.REST.DTOs.User.Requests
{
    public class UserCreateRequest
    {
        public required string Username { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
    }
}
