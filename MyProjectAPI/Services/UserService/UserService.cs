using MyProjectAPI.Models;

namespace MyProjectAPI.Services.UserService
{
  public class UserService : IUserService
  {
    public Task<User> GetUserByUsername(string username)
    {
      throw new NotImplementedException();
    }
  }
}