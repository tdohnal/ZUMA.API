using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZUMA.API.REST.Controllers.Base;
using ZUMA.API.REST.DTOs.Registration;
using ZUMA.API.REST.DTOs.Registration.Extensions;
using ZUMA.API.REST.DTOs.Registration.Requests;
using ZUMA.API.REST.Filters;
using ZUMA.BussinessLogic.Services.User;

namespace ZUMA.API.REST.Controllers
{
    public class RegistrationController : BaseController
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(
            IRegistrationService registrationService
            )
        {
            _registrationService = registrationService;
        }

        /// <summary>
        /// Adds a new Registration.
        /// </summary>
        [HttpPost("registrate")]
        [ApiVersion("1.0")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RegistrationDto))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> NewRegistrationAsync([FromBody] CreateRegistrationRequestDto request, CancellationToken cancellationToken = default)
        {
            var created = await _registrationService.CreateAsync(request.ToEntity(), cancellationToken);
            return Created();
        }
    }
}
