using MyProjectAPI.Models;

namespace MyProjectAPI.Services.UserService
{
  public interface IUserService
  {
    Task<User> GetUserByUsername(string username);
  }
}
