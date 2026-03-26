namespace ZUMA.BussinessLogic.Entities;

public interface IAuditableEntities
{
    long InternalId { get; set; }
    Guid PublicId { get; set; }

    DateTime Created { get; set; }
    DateTime? Updated { get; set; }
    DateTime? Deleted { get; set; }
}
