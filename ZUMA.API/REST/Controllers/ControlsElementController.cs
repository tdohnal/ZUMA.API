using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ZUMA.API.REST.Controllers.Base;
using ZUMA.API.REST.DTOs.ControlsElement;
using ZUMA.API.REST.Mappers;
using ZUMA.SharedKernel.MessagingContracts.Contracts.ControlsElement;

namespace ZUMA.API.REST.Controllers;

public class ControlsElementController : AuthorizedBaseController
{
    private readonly IMessageService _messageService;
    private readonly MessageMapper _mapper;

    public ControlsElementController(IMessageService messageService, MessageMapper mapper)
    {
        _messageService = messageService;
        _mapper = mapper;
    }

    #region V1

    /// <summary>
    /// Gets a ControlsElement by their ID.
    /// </summary>
    [HttpGet("{publicId}")]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ControlsElementDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetControlsElementByIdAsync(Guid publicId, CancellationToken cancellationToken = default)
    {
        var result = await _messageService.SendAsync<SendGetControlsElementByIdRequest, SendGetControlsElementByIdSuccess, SendControlsElementFailed>(
            new SendGetControlsElementByIdRequest { PublicId = publicId },
            cancellationToken);

        return result.ToOk(_mapper.MapSendGetControlsElementByIdSuccessToDto);
    }

    /// <summary>
    /// Get all ControlsElements
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ControlsElementDto>))]
    public async Task<IActionResult> GetControlsElementsAsync(CancellationToken cancellationToken = default)
    {
        var result = await _messageService.SendAsync<SendGetControlsElementsRequest, SendGetControlsElementsSuccess, SendControlsElementFailed>(
            new SendGetControlsElementsRequest(),
            cancellationToken);

        return result.ToOk(_mapper.MapSendGetControlsElementsSuccessToDtoList);
    }

    /// <summary>
    /// Adds a new ControlsElement.
    /// </summary>
    [HttpPost]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddControlsElementAsync([FromBody] ControlsElementCreateRequest request, CancellationToken cancellationToken = default)
    {
        // Namapujeme DTO na Message a ručně dohodíme OwnerId z tokenu
        var sendRequest = _mapper.MapCreateRequestToSendRequest(request, AuthorizedUserId);
        sendRequest.OwnerUserPublicId = AuthorizedUserId;

        var result = await _messageService.SendAsync<SendCreateControlsElementRequest, SendCreateControlsElementSuccess, SendControlsElementFailed>(
            sendRequest,
            cancellationToken);

        return result.ToCreated();
    }

    /// <summary>
    /// Updates an existing ControlsElement.
    /// </summary>
    [HttpPut("{publicId}")]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ControlsElementDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateControlsElementAsync(Guid publicId, [FromBody] ControlsElementDto request, CancellationToken cancellationToken = default)
    {
        var sendRequest = _mapper.MapUpdateRequestToSendRequest(request);
        sendRequest.PublicId = publicId;

        var result = await _messageService.SendAsync<SendUpdateControlsElementRequest, SendUpdateControlsElementSuccess, SendControlsElementFailed>(
            sendRequest,
            cancellationToken);

        return result.ToOk(_mapper.MapSendUpdateControlsElementSuccessToDto);
    }

    /// <summary>
    /// Deletes a ControlsElement with the specified ID.
    /// </summary>
    [HttpDelete("{publicId}")]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteControlsElementAsync(Guid publicId, CancellationToken cancellationToken = default)
    {
        var result = await _messageService.SendAsync<SendDeleteControlsElementRequest, SendDeleteControlsElementSuccess, SendControlsElementFailed>(
            new SendDeleteControlsElementRequest { PublicId = publicId },
            cancellationToken);

        return result.ToNoContent();
    }

    #endregion
}