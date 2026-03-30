using ZUMA.BussinessLogic.Messagges.Base;

namespace ZUMA.BussinessLogic.Messagges.Contracts.Users;


#region Get User By ID
public record SendGetUserByIdRequest : IRequestEvent
{
    public Guid PublicId { get; set; }
}
public record SendGetUserByIdSuccess : ISuccessResponse
{
    public UserMessageModel User { get; set; }
}
#endregion

#region Get All Users
public record SendGetUsersRequest : IRequestEvent
{

}
public record SendGetUsersSuccess : ISuccessResponse
{
    public List<UserMessageModel> User { get; set; }
}

public class UserMessageModel
{
    public Guid PublicId { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Deleted { get; set; }
}

#endregion

#region Create User
public record SendCreateUserRequest : IRequestEvent
{

}
public record SendCreateUserSuccess : ISuccessResponse
{

}
#endregion

#region Update User
public record SendUpdateUserRequest : IRequestEvent
{

}
public record SendUpdateUserSuccess : ISuccessResponse
{

}
#endregion

#region Delete User
public record SendDeleteUserRequest : IRequestEvent
{

}
public record SendDeleteUserSuccess : ISuccessResponse
{

}
#endregion

#region Common Failed Response
public record SendUserFailed : IFailedResponse
{
    public string ErrorMessage { get; set; }
    public string ErrorCode = "INTERNAL_ERROR";

}
#endregion

