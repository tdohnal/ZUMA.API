namespace ZUMA.BussinessLogic.Infrastructure.Entities;

public interface IAuditableEntities
{
    long Id { get; set; }

    DateTime Created { get; set; }
    DateTime? Updated { get; set; }
    DateTime? Deleted { get; set; }
}
