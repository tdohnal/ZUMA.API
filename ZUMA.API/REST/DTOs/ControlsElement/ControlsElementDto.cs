using ZUMA.API.REST.DTOs.ControlElementsItem;
using ZUMA.SharedKernel.Domain.ValueObjects.Customer.ControlsElement;
using ZUMA.SharedKernel.Enums;

namespace ZUMA.API.REST.DTOs.ControlsElement
{
    public class ControlsElementDto
    {
        #region Base

        public Guid PublicId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Deleted { get; set; }

        #endregion

        public required string Title { get; set; }

        public required Guid OwnerUserPublicId { get; set; }

        public required ListType ListType { get; set; }

        public List<ControlsElementsItemDto> Items { get; set; } = new();
        public ElementsPermission ElementsPermission { get; set; } = new();
    }
}
