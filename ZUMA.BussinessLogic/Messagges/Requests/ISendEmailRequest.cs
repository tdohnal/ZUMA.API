namespace ZUMA.BussinessLogic.Messagges.Requests
{
    public interface ISendEmailRequest
    {
        string Recipient { get; }
        string Subject { get; }
        string Body { get; }
    }
}
