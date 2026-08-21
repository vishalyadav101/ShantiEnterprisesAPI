using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
    }
}