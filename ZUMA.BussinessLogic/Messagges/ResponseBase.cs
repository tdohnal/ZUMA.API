namespace ZUMA.BussinessLogic.Messagges;

public class SuccessResponseBase
{
}

public class FailedResponseBase
{
    public string ErrorMessage { get; set; } = string.Empty;

}
