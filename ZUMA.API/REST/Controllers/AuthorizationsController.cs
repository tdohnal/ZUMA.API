using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ZUMA.API.REST.Controllers.Base;
using ZUMA.API.REST.DTOs.Authorization.Requests;
using ZUMA.API.REST.DTOs.Authorization.Responses;
using ZUMA.API.REST.DTOs.Registration.Requests;
using ZUMA.API.REST.Mappers;
using ZUMA.BussinessLogic.Messagges.Requests.Authorization.Request.SendRegistrationCreateRequest;
using ZUMA.BussinessLogic.Messagges.Requests.Authorization.Response;
using ZUMA.BussinessLogic.Messagges.Reuqests.Authorize.Request;
using ZUMA.BussinessLogic.Messagges.Reuqests.Authorize.Response;

namespace ZUMA.API.REST.Controllers
{
    public class AuthorizationController : BaseController
    {
        private readonly IMessageService _messageService;
        private readonly MessageMapper _mapper;

        public AuthorizationController(IMessageService messageService, MessageMapper mapper)
        {
            _messageService = messageService;
            _mapper = mapper;
        }

        #region v1

        /// <summary>
        /// Handles user registration by processing the provided registration request and returning the appropriate
        /// response based on the outcome.
        /// </summary>
        /// <remarks>This method asynchronously sends a registration request and maps the response to the
        /// appropriate success or failure types. Ensure that the request data is valid before calling this
        /// method.</remarks>
        /// <param name="request">The registration request data containing user information required for registration.</param>
        /// <returns>An IActionResult indicating the result of the registration process, which can be a success response with a
        /// 201 status code or a failure response with a 422 status code.</returns>
        [HttpPost("registrate")]
        [ProducesResponseType(typeof(RegistrateSuccess), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(RegistrateFailed), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> NewRegistrationAsync([FromBody] CreateRegistrationRequestDto request)
        {
            var message = _mapper.MapCreateRegistrationDtoToSendRequest(request);
            var result = await _messageService.SendAsync<SendRegistrationCreateRequest, RegistrateSuccess, RegistrateFailed>(message);

            return result.ToCreated();
        }

        /// <summary>
        /// Handles an authorization request by sending an authorization email to the user associated with the specified
        /// email address.
        /// </summary>
        /// <remarks>This method checks whether the provided email address exists in the system. If a
        /// matching user is found, an authorization code is generated and sent to the user's email address. If no user
        /// is found, a not found response is returned. Ensure that the email address provided in the request is valid
        /// and registered in the system.</remarks>
        /// <param name="request">The authorization request containing the email address of the user to authorize. Cannot be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>An IActionResult that indicates the result of the authorization process. Returns Status200OK if the
        /// authorization email is sent successfully; returns Status404NotFound if the email address is not associated
        /// with any user.</returns>
        [HttpPost("authorizeByEmail")]
        [ApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Authorize(AuthorizationRequest request, CancellationToken cancellationToken = default)
        {
            var message = _mapper.MapAuthorizationDtoToSendRequest(request);
            var result = await _messageService.SendAsync<SendAuthorizeUserRequest, AuthorizeUserSuccess, AuthorizeUserFailed>(message);

            return result.ToOk();
        }

        [HttpPost("verification")]
        [ApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VerificationResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerificateAuthorizationCode(VerificationRequest request, CancellationToken cancellationToken = default)
        {
            var message = _mapper.MapVerificationDtoToSendRequest(request);
            var result = await _messageService.SendAsync<SendVerifyCodeRequest, VerificationSuccess, VerificationFailed>(message);

            return result.ToOk(_mapper.MapVerificationSuccessToResponse);
        }

        #endregion;
    }
}
