namespace ZUMA.BussinessLogic.Messagges.Registrate.Request
{
    public interface ISendRegistrationCreateRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
