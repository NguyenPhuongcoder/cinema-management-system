using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IUserService
    {
        User GetUserById(int userId);
        void CreateUser(User user);
        void UpdateUser(User user);
    }
} 