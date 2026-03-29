namespace ZUMA.BussinessLogic.Messagges.Events;

public class FireEmailEvent
{
    public Guid EmailId { get; set; }
    public string Email { get; set; }
}
