using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZUMA.CustomerService.Domain.ValueObjects;
using ZUMA.SharedKernel.Entities;

namespace ZUMA.CustomerService.Domain.Entities;

[Table("Users")]
public class UserEntity : IAuditableEntities
{
    [Key]
    public long Id { get; set; }
    public Guid PublicId { get; set; } = Guid.CreateVersion7();

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string? AuthCode { get; set; } = string.Empty;
    public Address? Address { get; set; }

    public DateTime? AuthCodeExpiration { get; set; }

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Deleted { get; set; }
}
