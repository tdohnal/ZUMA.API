namespace ZUMA.BussinessLogic.Messagges.Verification.Response
{
    public class VerificationSuccess : SuccessResponseBase
    {
        public Guid PublicId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Deleted { get; set; }
        public string Token { get; set; }
    }

    public class VerificationFailed : FailedResponseBase
    {

    }
}
