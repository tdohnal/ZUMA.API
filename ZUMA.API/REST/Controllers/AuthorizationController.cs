using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ZUMA.API.REST.Controllers.Base;
using ZUMA.API.REST.DTOs.Authorization.Requests;
using ZUMA.API.REST.DTOs.Authorization.Responses;
using ZUMA.BussinessLogic.Services.User;

namespace ZUMA.API.REST.Controllers
{
    public class AuthorizationController : BaseController
    {
        private readonly IUserService _userService;

        public AuthorizationController(IUserService userService)
        {
            _userService = userService;
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
            var ret = await _userService.GetIdByEmailAsync(request.Email, cancellationToken);
            if (ret == null)
            {
                return NotFound("Authorization email was sent");
            }

            await _userService.GetAuthorizationCodeAsync(ret.Value, cancellationToken);

            return Ok();
        }

        [HttpPost("verification")]
        [ApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VerificationResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerificateAuthorizationCode(VerificationRequest request, CancellationToken cancellationToken = default)
        {
            var ret = await _userService.VerificateAuthorizationCode(request.Code, request.Email, cancellationToken);

            if (ret.IsValid == false)
            {
                return NotFound(ret.ErrorMessage);
            }

            return Ok(new VerificationResponse
            {
                Token = ret.Token,
                User = new DTOs.User.UserDto
                {
                    Created = ret.Entity.Created,
                    Email = ret.Entity.Email,
                    Name = ret.Entity.FullName,
                    PublicId = ret.Entity.PublicId,
                    UserName = ret.Entity.UserName,
                }
            });
        }

        #endregion;
    }
}
