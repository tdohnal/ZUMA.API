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
        /// Handles an authorization request by validating the provided user credentials and returning the result of the
        /// authorization process.
        /// </summary>
        /// <remarks>This method is intended to be called via an HTTP POST request to the 'authorize'
        /// endpoint. Ensure that the request contains valid credentials to avoid authorization failure.</remarks>
        /// <param name="request">The authorization request containing user credentials and any additional information required for
        /// authorization. Cannot be null.</param>
        /// <returns>An <see cref="IActionResult"/> that represents the result of the authorization operation. Returns a 200 OK
        /// response with an <see cref="AuthorizationResponseDto"/> if authorization is successful; otherwise, returns a
        /// 404 Not Found response.</returns>
        [HttpPost("authorize")]
        [ApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorizationResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Authorize(AuthorizationRequestDto request, CancellationToken cancellationToken = default)
        {
            var ret = await _userService.GetIdByEmailAsync(request.Email, cancellationToken);
            if (ret == null)
            {
                return NotFound();
            }

            var code = await _userService.GetAuthorizationCodeAsync(ret.Value, cancellationToken);

            return Ok(new AuthorizationResponseDto(code));
        }

        #endregion;
    }
}
