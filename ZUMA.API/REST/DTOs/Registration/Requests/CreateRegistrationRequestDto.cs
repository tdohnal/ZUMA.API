namespace ZUMA.API.REST.DTOs.Registration.Requests
{
    public sealed record CreateRegistrationRequestDto
        (
        string FirstName,
            string LastName,
            string Email,
            string Username
        );

}
