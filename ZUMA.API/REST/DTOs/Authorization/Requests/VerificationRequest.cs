using System.ComponentModel.DataAnnotations;

namespace ZUMA.API.REST.DTOs.Authorization.Requests
{
    public class VerificationRequest
    {
        [MaxLength(12)]
        public string Code { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
