namespace ZUMA.BussinessLogic.Messagges.Requests.Authorization.Request.SendRegistrationCreateRequest;

// Místo interface použij record
public record SendRegistrationCreateRequest(
    string FirstName,
    string LastName,
    string Email,
    string Username
) : ISendRegistrationCreateRequest;