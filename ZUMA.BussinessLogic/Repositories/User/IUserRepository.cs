using ZUMA.BussinessLogic.Entities.Customer;

namespace ZUMA.BussinessLogic.Repositories.User;

public interface IUserRepository : IRepositoryBase<UserEntity>
{
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
