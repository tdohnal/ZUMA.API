using Riok.Mapperly.Abstractions;
using ZUMA.API.REST.DTOs.ControlsElement;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.ControlsElement;

namespace ZUMA.API.Mappers;

[Mapper]
public partial class ControlsElementMapper
{
    // --- REQUEST MAPPING ---

    [MapProperty(nameof(ownerUserPublicId), nameof(SendCreateControlsElementRequest.OwnerUserPublicId))]
    public partial SendCreateControlsElementRequest MapCreateRequestToSendRequest(
        ControlsElementCreateRequest dto,
        Guid ownerUserPublicId);

    [MapperIgnoreTarget(nameof(SendUpdateControlsElementRequest.PublicId))]
    public partial SendUpdateControlsElementRequest MapUpdateRequestToSendRequest(
        ControlsElementDto dto);


    // --- RESPONSE MAPPING ---

    public partial ControlsElementDto MapControlsElementMessageModelToDto(ControlsElementMessageModel model);

    private partial List<ControlsElementDto> MapControlsElementList(List<ControlsElementMessageModel> items);

    public List<ControlsElementDto> MapSendGetControlsElementsSuccessToDtoList(SendGetControlsElementsSuccess success) =>
        success?.ControlsElement == null ? [] : MapControlsElementList(success.ControlsElement);

    public ControlsElementDto MapSendGetControlsElementByIdSuccessToDto(SendGetControlsElementByIdSuccess success) =>
        MapControlsElementMessageModelToDto(success.ControlsElement);

    public ControlsElementDto MapSendCreateControlsElementSuccessToDto(SendCreateControlsElementSuccess success) =>
        MapControlsElementMessageModelToDto(success.ControlsElement);

    public ControlsElementDto MapSendUpdateControlsElementSuccessToDto(SendUpdateControlsElementSuccess success) =>
        MapControlsElementMessageModelToDto(success.ControlsElement);
}