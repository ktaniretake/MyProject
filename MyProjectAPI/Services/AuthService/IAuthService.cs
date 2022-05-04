using MyProjectAPI.Models;
namespace MyProjectAPI.Services.AuthService
{
  public interface IAuthService
  {
    Task<User> TryRegisterUserAsync(UserDto userDto);
    Task<bool> LoginAsync(UserDto userDto, out string? token);
    //void LogoutAsync();
  }
}