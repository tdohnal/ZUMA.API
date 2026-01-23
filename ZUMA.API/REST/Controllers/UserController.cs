using Microsoft.AspNetCore.Mvc;
using ZUMA.API.REST.Base;

namespace ZUMA.API.REST.Controllers;


public class UserController : ZumaBaseController
{

    private readonly IBusiness<UserDto, UserEntity> _UserBusiness;

    public UserController(IBusiness<UserDto, UserEntity> UserBusiness)
    {
        _UserBusiness = UserBusiness;
    }

    /// <summary>
    /// Gets a User by their ID.
    /// </summary>
    /// <param name="id">The ID of the User to get.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the requested entity.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserEntity))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByIdAsync(int id)
    {
        var User = await _UserBusiness.GetById(id);
        return User == null ? NotFound() : Ok(User);
    }

    /// <summary>
    /// Adds a new User.
    /// </summary>
    /// <param name="UserDto">The User object to add.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the new entity created.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserEntity))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> AddUserAsync([FromBody] UserDto UserDto)
    {
        return Created(string.Empty, await _UserBusiness.Add(UserDto));
    }

    /// <summary>
    /// Updates an existing User.
    /// </summary>
    /// <param name="id">The ID of the User to update, passed in a header.</param>
    /// <param name="UserDTO">The updated User object.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the updated entity.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserEntity))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UserDto UserDTO)
    {
        var updatedUser = await _UserBusiness.Update(id, UserDTO);

        return updatedUser == null ? NotFound() : Ok(updatedUser);
    }
    /// <summary>
    /// Deletes a User with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the User to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserAsync(int id)
    {
        await _UserBusiness.Delete(id);
        return NoContent();
    }
}
