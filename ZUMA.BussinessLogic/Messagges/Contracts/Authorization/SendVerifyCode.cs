using ZUMA.BussinessLogic.Messagges.Base;

namespace ZUMA.BussinessLogic.Messagges.Contracts.Authorization;

public class SendVerifyCodeRequest : IRequestEvent
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

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

public record VerificationSuccess : ISuccessResponse
{
    public VerificationUserMessage User { get; set; }
    public string Token { get; set; }
}

public class VerificationFailed : IFailedResponse
{


}