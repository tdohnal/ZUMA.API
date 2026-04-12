using ZUMA.API.REST.DTOs.ControlElementsItem;
using ZUMA.SharedKernel.Domain.ValueObjects.Customer.ControlsElement;
using ZUMA.SharedKernel.Enums;

namespace ZUMA.API.REST.DTOs.ControlsElement
{
    public class ControlsElementCreateRequest
    {
        public required string Title { get; set; }

        public required ListType ListType { get; set; }

        public List<ControlsElementsItemCreateRequest> Items { get; set; } = new();
        public ElementsPermission ElementsPermission { get; set; } = new();
    }
}
