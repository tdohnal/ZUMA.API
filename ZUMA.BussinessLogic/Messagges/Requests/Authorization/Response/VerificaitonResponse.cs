namespace ZUMA.BussinessLogic.Messagges.Requests.Authorization.Response
{
    public record VerificationUserMessage
    {
        public Guid PublicId { get; init; }
        public string UserName { get; init; }
        public string FullName { get; init; }
        public string Email { get; init; }
        public DateTime Created { get; init; }
        public DateTime? Updated { get; init; }
        public DateTime? Deleted { get; init; }
    }

    public record VerificationSuccess
    {
        public VerificationUserMessage User { get; set; }
        public string Token { get; set; }
    }

    public class VerificationFailed : FailedResponseBase
    {


    }
}
