using ZUMA.BussinessLogic.Messagges.Base;

namespace ZUMA.BussinessLogic.Messagges.Events;

public class FireEmailEvent : IEvent
{
    public Guid EmailId { get; set; }
    public string Email { get; set; }
}
