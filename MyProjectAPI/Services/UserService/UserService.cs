using MyProjectAPI.Models;

namespace MyProjectAPI.Services.UserService
{
  public class UserService : IUserService
  {
    private readonly DataContext _context;

    public UserService(DataContext context)
    {
      _context = context;
    }
    public async Task<User?> GetUserByIdAsync(int id)
    {
      var user = await _context.Users.FindAsync(id);

      await _context.SaveChangesAsync();
      return user;
    }
  }
}