using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZUMA.API.REST.Controllers.Base;
using ZUMA.API.REST.DTOs.Registration.Requests;
using ZUMA.API.REST.Filters;
using ZUMA.BussinessLogic.Messagges.Registrate.Request;
using ZUMA.BussinessLogic.Messagges.Registrate.Response;

namespace ZUMA.API.REST.Controllers
{
    public class RegistrationController : BaseController
    {
        private readonly IRequestClient<ISendRegistrationCreateRequest> _registerClient;

        public RegistrationController(
            IRequestClient<ISendRegistrationCreateRequest> registerClient
            )
        {
            _registerClient = registerClient;
        }

        /// <summary>
        /// Adds a new Registration.
        /// </summary>
        [HttpPost("registrate")]
        [ApiVersion("1.0")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> NewRegistrationAsync([FromBody] CreateRegistrationRequestDto request, CancellationToken cancellationToken = default)
        {
            var response = await _registerClient.GetResponse<RegistrateSuccess, RegistrateFailed>(new
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Username = request.Username,
            }, cancellationToken);

            if (response.Is(out Response<RegistrateSuccess> success))
            {
                return Created();
            }

            return NotFound(new { message = response.Message }); // Vrátí 404
        }
    }
}
