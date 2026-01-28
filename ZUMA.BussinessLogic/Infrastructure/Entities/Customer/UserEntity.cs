using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZUMA.BussinessLogic.Infrastructure.Entities.Customer;

[Table("Users")]
public class UserEntity : IAuditableEntities
{
    [Key]
    public long Id { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string HashedPassword { get; set; }

    public string UserName { get; set; }

    public bool IsConfirmed { get; set; }

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Deleted { get; set; }
}
