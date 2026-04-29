using Riok.Mapperly.Abstractions;
using ZUMA.API.REST.DTOs.Authorization.Requests;
using ZUMA.API.REST.DTOs.Authorization.Responses;
using ZUMA.API.REST.DTOs.ControlsElement;
using ZUMA.API.REST.DTOs.Registration.Requests;
using ZUMA.API.REST.DTOs.User;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Authorization;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.ControlsElement;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Users;

namespace ZUMA.API.REST.Mappers;

[Mapper]
public partial class MessageMapper
{
    #region Users (Requests)


    public partial UserDto MapUserMessageModelToUserDto(UserMessageModel model);
    private partial List<UserDto> MapUserList(List<UserMessageModel> users);
    public List<UserDto> MapSendGetUsersSuccessToUserDto(SendGetUsersSuccess success)
    {
        if (success?.User == null) return [];

        return MapUserList(success.User);
    }

    public partial UserDto MapSendGetUserByIdSuccessToUserDto(SendGetUserByIdSuccess success);

    public partial UserDto MapSendCreateUserSuccessToUserDto(SendCreateUserSuccess success);

    public partial UserDto MapSendUpdateUserSuccessToUserDto(SendUpdateUserSuccess success);

    #endregion

    #region Authorization (Requests)

    public partial SendRegistrationCreateRequest MapCreateRegistrationDtoToSendRequest(CreateRegistrationRequestDto dto);
    public partial SendAuthorizeUserRequest MapAuthorizationDtoToSendRequest(AuthorizationRequest dto);
    public partial SendVerifyCodeRequest MapVerificationDtoToSendRequest(VerificationRequest dto);

    #endregion

    #region Authorization (Responses)

    public partial VerificationResponse MapVerificationSuccessToResponse(VerificationSuccess success);

    #endregion

    #region Registration (Responses)


    #endregion

    #region ControlsElement

    public partial SendCreateControlsElementRequest MapCreateRequestToSendRequest(ControlsElementCreateRequest dto, Guid ownerUserPublicId);
    public partial SendUpdateControlsElementRequest MapUpdateRequestToSendRequest(ControlsElementDto dto);

    // Mapování odpovědí (Responses)
    public partial ControlsElementDto MapControlsElementMessageModelToDto(ControlsElementMessageModel model);
    private partial List<ControlsElementDto> MapControlsElementList(List<ControlsElementMessageModel> items);

    public List<ControlsElementDto> MapSendGetControlsElementsSuccessToDtoList(SendGetControlsElementsSuccess success)
    {
        return success?.ControlsElement == null ? [] : MapControlsElementList(success.ControlsElement);
    }

    public ControlsElementDto MapSendGetControlsElementByIdSuccessToDto(SendGetControlsElementByIdSuccess success) =>
        MapControlsElementMessageModelToDto(success.ControlsElement);

    public ControlsElementDto MapSendCreateControlsElementSuccessToDto(SendCreateControlsElementSuccess success) =>
        MapControlsElementMessageModelToDto(success.ControlsElement);

    public ControlsElementDto MapSendUpdateControlsElementSuccessToDto(SendUpdateControlsElementSuccess success) =>
        MapControlsElementMessageModelToDto(success.ControlsElement);

    #endregion
}