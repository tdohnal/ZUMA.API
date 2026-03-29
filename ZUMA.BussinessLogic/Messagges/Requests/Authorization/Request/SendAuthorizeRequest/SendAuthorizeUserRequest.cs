using ZUMA.BussinessLogic.Messagges.Reuqests.Authorization.Request.SendAuthorizeRequest;

namespace ZUMA.BussinessLogic.Messagges.Reuqests.Authorize.Request;

public class SendAuthorizeUserRequest : ISendAuthorizeUserRequest
{
    public string Email { get; set; } = null!;
}
