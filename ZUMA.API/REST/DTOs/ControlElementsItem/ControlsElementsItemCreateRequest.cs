namespace ZUMA.API.REST.DTOs.ControlElementsItem;

public class ControlsElementsItemCreateRequest
{
    public Guid ControlElementPublicId { get; set; }
    public required string Content { get; set; }
    public string? Metadata { get; set; }
}
