using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ZUMA.API.REST.Controllers.Base;
using ZUMA.API.REST.DTOs.User;
using ZUMA.API.REST.Filters;
using ZUMA.BussinessLogic.Services.User;

namespace ZUMA.API.REST.Controllers;

public class UserController : AuthorizedBaseController
{
    private readonly IUserService _userService;

    public UserController(
        IUserService userService
        )
    {
        _userService = userService;
    }

    #region V1

    /// <summary>
    /// Gets a User by their ID.
    /// </summary>
    [HttpGet("{id}")]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        return user == null ? NotFound() : Ok(user);
    }

    /// <summary>
    /// Get all Users
    /// </summary>
    [HttpGet()]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetAllAsync(cancellationToken);
        return user == null ? NotFound() : Ok(user);
    }

    /// <summary>
    /// Adds a new User.
    /// </summary>
    [HttpPost]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(long))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> AddUserAsync([FromBody] UserDto userDto, CancellationToken cancellationToken = default)
    {
        //var created = await _userService.CreateAsync(userDto.ToEntity(), cancellationToken);
        //return created != null ? Ok(created.Id) : NoContent();
        return Ok();
    }

    /// <summary>
    /// Updates an existing User.
    /// </summary>
    [HttpPut("{id}")]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UserDto userDto, CancellationToken cancellationToken = default)
    {
        //var entity = userDto.ToEntity();
        //entity.Id = id; // zajistí update správného záznamu

        //var updated = await _userService.UpdateAsync(entity, cancellationToken);
        //return updated == null ? NotFound() : Ok(updated);
        return Ok();
    }

    /// <summary>
    /// Deletes a User with the specified ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var success = await _userService.DeleteAsync(id, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    #endregion

}
