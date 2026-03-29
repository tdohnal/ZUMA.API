using Riok.Mapperly.Abstractions;
using ZUMA.API.REST.DTOs.Authorization.Requests;
using ZUMA.API.REST.DTOs.Authorization.Responses;
using ZUMA.API.REST.DTOs.Registration.Requests;
using ZUMA.API.REST.DTOs.User;
using ZUMA.BussinessLogic.Messagges.Requests.Authorization.Request.SendRegistrationCreateRequest;
using ZUMA.BussinessLogic.Messagges.Requests.Authorization.Response;
using ZUMA.BussinessLogic.Messagges.Reuqests.Authorize.Request;

namespace ZUMA.API.REST.Mappers;

[Mapper]
public partial class MessageMapper
{
    #region Authorization (Requests)

    public partial SendRegistrationCreateRequest MapCreateRegistrationDtoToSendRequest(CreateRegistrationRequestDto dto);
    public partial SendAuthorizeUserRequest MapAuthorizationDtoToSendRequest(AuthorizationRequest dto);
    public partial SendVerifyCodeRequest MapVerificationDtoToSendRequest(VerificationRequest dto);

    #endregion

    #region Authorization (Responses)

    // Hlavní mapování pro endpoint
    public partial VerificationResponse MapVerificationSuccessToResponse(VerificationSuccess success);


    [MapProperty(nameof(VerificationUserMessage.FullName), nameof(UserDto.Name))]
    private partial UserDto MapVerificationUserMessageToUserDto(VerificationUserMessage message);

    #endregion

    #region Registration (Responses)


    #endregion
}