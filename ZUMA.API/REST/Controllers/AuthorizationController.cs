using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using ZUMA.API.Messages;
using ZUMA.API.REST.Controllers.Base;
using ZUMA.API.REST.DTOs.Authorization.Requests;
using ZUMA.API.REST.DTOs.Authorization.Responses;
using ZUMA.BussinessLogic.Messagges.Authorize.Request;
using ZUMA.BussinessLogic.Messagges.Verification.Response;

namespace ZUMA.API.REST.Controllers
{
    public class AuthorizationController : BaseController
    {
        private readonly IRequestClient<ISendVerifyCodeRequest> _verifyClient;
        private readonly IRequestClient<ISendAuthorizeUserRequest> _authorizeClient;

        public AuthorizationController(
            IRequestClient<ISendVerifyCodeRequest> verifyClient,
            IRequestClient<ISendAuthorizeUserRequest> authorizeClient
            )
        {
            _verifyClient = verifyClient;
            _authorizeClient = authorizeClient;
        }

        #region v1

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
        public async Task<IActionResult> Authorize(AuthorizationRequestDto request, CancellationToken cancellationToken = default)
        {
            var response = await _authorizeClient.GetResponse<AuthorizeUserSuccess, AuthorizeUserFailed>(new
            {
                Email = request.Email,
            });

            if (response.Is(out Response<AuthorizeUserSuccess> success))
            {
                return Ok();
            }

            return NotFound(new { message = response.Message }); // Vrátí 404
        }

        [HttpPost("verification")]
        [ApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VerificationResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerificateAuthorizationCode(VerificationRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _verifyClient.GetResponse<VerificationSuccess, VerificationFailed>(new
            {
                Email = request.Email,
                Code = request.Code,
            });

            if (response.Is(out Response<VerificationSuccess> success))
            {
                return Ok(new VerificationResponse
                {
                    User = new DTOs.User.UserDto
                    {
                        PublicId = success.Message.PublicId,
                        UserName = success.Message.UserName,
                        Name = success.Message.Name,
                        Email = success.Message.Email,
                        Created = success.Message.Created,
                        Updated = success.Message.Updated,
                        Deleted = success.Message.Deleted,
                    },
                    Token = success.Message.Token,
                });
            }

            return NotFound(new { message = "Neplatný kód nebo uživatel nenalezen" }); // Vrátí 404
        }

        #endregion;
    }
}
