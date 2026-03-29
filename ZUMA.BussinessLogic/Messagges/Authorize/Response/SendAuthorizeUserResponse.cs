using ZUMA.BussinessLogic.Messagges;

namespace ZUMA.API.Messages
{
    public class AuthorizeUserSuccess : SuccessResponseBase
    {
        public DateTime SentAt { get; set; }
    }

    public class AuthorizeUserFailed : FailedResponseBase
    {
    }
}
