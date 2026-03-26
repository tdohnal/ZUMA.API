using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZUMA.BussinessLogic.Entities.Customer
{
    [Table("Registrations")]
    public class RegistrationEntity : IAuditableEntities
    {
        [Key]
        public long InternalId { get; set; }
        public Guid PublicId { get; set; } = Guid.CreateVersion7();

        public string? ActivationCode { get; set; }

        public DateTime ExpirationCodeDate { get; set; }

        public long UserId { get; set; }

        public virtual UserEntity User { get; set; } = null!;

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Deleted { get; set; }

    }
}
