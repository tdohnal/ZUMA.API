using ZUMA.API.REST.DTOs.ControlElementsItem;
using ZUMA.SharedKernel.Domain.Enums;
using ZUMA.SharedKernel.Domain.ValueObjects.Customer.ControlsElement;

namespace ZUMA.API.REST.DTOs.ControlsElement
{
    public class ControlsElementCreateRequest
    {
        public required string Title { get; set; }

        public required ListType ListType { get; set; }

        public List<ControlsElementsItemCreateRequest> Items { get; set; } = [];
        public ElementsPermission ElementsPermission { get; set; } = new();
    }
}
