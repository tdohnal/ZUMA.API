using ZUMA.BussinessLogic.Repositories;
using ZUMA.CustomerService.Entities;

namespace ZUMA.CustomerService.Repositories.User;

public interface IUserRepository : IRepositoryBase<UserEntity>
{
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
