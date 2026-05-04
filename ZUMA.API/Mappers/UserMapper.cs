using Riok.Mapperly.Abstractions;
using ZUMA.API.REST.DTOs.User;
using ZUMA.API.REST.DTOs.User.Requests;
using ZUMA.SharedKernel.Domain.MessagingContracts.Contracts.Users;

namespace ZUMA.API.Mappers;

[Mapper]
public partial class UserMapper
{
    [MapperIgnoreTarget(nameof(SendCreateUserRequest.PublicId))]
    public partial SendCreateUserRequest MapCreateUserRequestToSendRequest(UserCreateRequest dto, HttpMethod method);

    public partial SendUpdateUserRequest MapUpdateUserRequestToSendRequest(UserCreateRequest dto, HttpMethod method);

    public partial UserDto MapUserMessageModelToUserDto(UserMessageModel model);

    private partial List<UserDto> MapUserList(List<UserMessageModel> users);

    public List<UserDto> MapSendGetUsersSuccessToUserDto(SendGetUsersSuccess success)
    {
        return success?.User == null ? [] : MapUserList(success.User);
    }

    public partial UserDto MapSendGetUserByIdSuccessToUserDto(SendGetUserByIdSuccess success);
    public partial UserDto MapSendUpdateUserSuccessToUserDto(SendUpdateUserSuccess success);
}