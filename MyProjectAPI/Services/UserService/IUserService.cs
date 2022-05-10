using MyProjectAPI.Models;

namespace MyProjectAPI.Services.UserService
{
  public interface IUserService
  {
    Task<User?> GetUserByIdAsync(int id);
  }
}
