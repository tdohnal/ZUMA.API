namespace ZUMA.API.REST.DTOs.ControlElementsItem;

public class ControlsElementsItemDto
{
    public Guid PublicId { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Deleted { get; set; }
    public Guid ControlElementPublicId { get; set; }
    public required string Content { get; set; }
    public string? Metadata { get; set; }
}
