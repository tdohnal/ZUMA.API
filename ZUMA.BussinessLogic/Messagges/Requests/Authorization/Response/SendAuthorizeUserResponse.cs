namespace ZUMA.BussinessLogic.Messagges.Reuqests.Authorize.Response
{
    public class AuthorizeUserSuccess : SuccessResponseBase
    {
        public DateTime SentAt { get; set; }
    }

    public class AuthorizeUserFailed : FailedResponseBase
    {
    }
}
