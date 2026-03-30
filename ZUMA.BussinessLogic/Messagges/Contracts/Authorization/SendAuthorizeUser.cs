using ZUMA.BussinessLogic.Messagges.Base;

namespace ZUMA.BussinessLogic.Messagges.Contracts.Authorization;

public class SendAuthorizeUserRequest : IRequestEvent
{
    public string Email { get; set; } = null!;
}

public class AuthorizeUserSuccess : ISuccessResponse
{
    public DateTime SentAt { get; set; }
}

public class AuthorizeUserFailed : IFailedResponse
{
}
