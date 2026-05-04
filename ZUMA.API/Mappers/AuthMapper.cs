using Riok.Mapperly.Abstractions;
using ZUMA.API.REST.DTOs.Authorization.Requests;
using ZUMA.API.REST.DTOs.Authorization.Responses;
using ZUMA.API.REST.DTOs.Registration.Requests;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Authorization;

namespace ZUMA.API.Mappers;

[Mapper]
public partial class AuthMapper
{

    public partial SendRegistrationCreateRequest MapCreateRegistrationDtoToSendRequest(CreateRegistrationRequestDto dto, HttpMethod method);


    public partial SendAuthorizeUserRequest MapAuthorizationDtoToSendRequest(AuthorizationRequest dto, HttpMethod method);


    public partial SendVerifyCodeRequest MapVerificationDtoToSendRequest(VerificationRequest dto, HttpMethod method);


    public partial VerificationResponse MapVerificationSuccessToResponse(VerificationSuccess success);
}