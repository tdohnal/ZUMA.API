using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ZUMA.API.REST.Controllers.Base;
using ZUMA.API.REST.DTOs.User;
using ZUMA.API.REST.Mappers;
using ZUMA.BussinessLogic.Messagges.Contracts.Users;

namespace ZUMA.API.REST.Controllers;

public class UserController : AuthorizedBaseController
{
    private readonly IMessageService _messageService;
    private readonly MessageMapper _mapper;

    public UserController(IMessageService messageService, MessageMapper mapper)
    {
        _messageService = messageService;
        _mapper = mapper;
    }

    #region V1

    /// <summary>
    /// Gets a User by their ID.
    /// </summary>
    [HttpGet("{publicId}")]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByIdAsync(Guid publicId, CancellationToken cancellationToken = default)
    {
        var result = await _messageService.SendAsync<SendGetUserByIdRequest, SendGetUserByIdSuccess, SendUserFailed>(new SendGetUserByIdRequest { PublicId = publicId }, cancellationToken);
        return result.ToOk(_mapper.MapSendGetUserByIdSuccessToUserDto);
    }

    /// <summary>
    /// Get all Users
    /// </summary>
    [HttpGet()]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var result = await _messageService.SendAsync<SendGetUsersRequest, SendGetUsersSuccess, SendUserFailed>(new SendGetUsersRequest(), cancellationToken);
        return result.ToOk(_mapper.MapSendGetUsersSuccessToUserDto);
    }

    ///// <summary>
    ///// Adds a new User.
    ///// </summary>
    //[HttpPost]
    //[ApiVersion("1.0")]
    //[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    //[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    //[ServiceFilter(typeof(ValidationFilterAttribute))]
    //public async Task<IActionResult> AddUserAsync([FromBody] UserCreateRequest request, CancellationToken cancellationToken = default)
    //{
    //    var entity = new UserEntity
    //    {
    //        FullName = request.FullName,
    //        UserName = request.Username,
    //        Email = request.Email
    //    };
    //    var created = await _userService.CreateAsync(entity, cancellationToken);
    //    return created != null ? Ok(created.PublicId) : NoContent();
    //}

    ///// <summary>
    ///// Updates an existing User.
    ///// </summary>
    //[HttpPut("{publicId}")]
    //[ApiVersion("1.0")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    //[ServiceFilter(typeof(ValidationFilterAttribute))]
    //public async Task<IActionResult> UpdateUserAsync(Guid publicIdid, [FromBody] UserDto userDto, CancellationToken cancellationToken = default)
    //{
    //    //var entity = userDto.ToEntity();
    //    //entity.Id = id; // zajistí update správného záznamu

    //    //var updated = await _userService.UpdateAsync(entity, cancellationToken);
    //    //return updated == null ? NotFound() : Ok(updated);
    //    return Ok();
    //}

    ///// <summary>
    ///// Deletes a User with the specified ID.
    ///// </summary>
    //[HttpDelete("{publicId}")]
    //[ApiVersion("1.0")]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> DeleteUserAsync(Guid publicId, CancellationToken cancellationToken = default)
    //{
    //    var ret = await _userService.GetByPublicIdAsync(publicId, cancellationToken);
    //    if (ret == null)
    //    {
    //        return NotFound();
    //    }

    //    var success = await _userService.DeleteAsync(ret.InternalId, cancellationToken);
    //    return success ? NoContent() : NotFound();
    //}

    #endregion

}
