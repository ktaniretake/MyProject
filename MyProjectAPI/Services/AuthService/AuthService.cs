using Microsoft.IdentityModel.Tokens;
using MyProjectAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MyProjectAPI.Services.AuthService
{
  public class AuthService : IAuthService
  {
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(DataContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    public async Task<User> TryRegisterUserAsync(UserDto userDto)
    {
      if (userDto == null)
        throw new ArgumentNullException(nameof(userDto));

      if (GetUserByUsername(userDto.Username) != null)
        throw new ArgumentException($"User with username have already existed");

      User user;

      if (userDto.IsAdmin == true && userDto.AdminsSecretKey == _configuration.GetSection("AppSettings:AdminSecretKey").Value)
      {
        user = CreateUser(userDto, Role.Admin);
      }
      else if (userDto.IsAdmin == true && userDto.AdminsSecretKey != _configuration.GetSection("AppSettings:AdminSecretKey").Value)
      {
        throw new FormatException("Secret key isn't valid");
      }
      else
      {
        user = CreateUser(userDto, Role.User);
      }

      await _context.SaveChangesAsync();

      return user;
    }

    public Task<bool> LoginAsync(UserDto userDto, out string? token)
    {
      User? user = GetUserByUsername(userDto.Username);

      if (user != null)
      {
        if (VerifyPasswordHash(user, userDto.Password, user.PasswordHash, user.PasswordSalt))
        {
          token = CreateToken(user);
          return Task.FromResult(true);
        }
      }

      token = null;
      return Task.FromResult(false);
    }

    private User? GetUserByUsername(string username)
    {
      if (string.IsNullOrEmpty(username))
        throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));

      return _context.Users.FirstOrDefault(x => x.Username == username);
    }

    private User CreateUser(UserDto userDto, Role role)
    {
      var user = new User();

      CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

      user.Username = userDto.Username;
      user.FirstName = userDto.FirstName;
      user.LastName = userDto.LastName;
      user.RoleId = role;
      user.RegistrationDate = DateTime.Now;
      user.PasswordHash = passwordHash;
      user.PasswordSalt = passwordSalt;

      _context.Users.Add(user);
      return user;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      if (string.IsNullOrEmpty(password))
        throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));

      using (var hmac = new HMACSHA512())
      {
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
      }
    }

    private bool VerifyPasswordHash(User user, string password, byte[] passwordHash, byte[] passwordSalt)
    {
      if (user is null)
        throw new ArgumentNullException(nameof(user));

      if (string.IsNullOrEmpty(password))
        throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));

      if (passwordHash is null)
        throw new ArgumentNullException(nameof(passwordHash));

      if (passwordSalt is null)
        throw new ArgumentNullException(nameof(passwordSalt));

      using (var hmac = new HMACSHA512(user.PasswordSalt))
      {
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
      }
    }

    private string CreateToken(User user)
    {
      if (user is null)
        throw new ArgumentNullException(nameof(user));

      List<Claim> claims = new List<Claim>
      {
        new Claim(ClaimTypes.Name, user.Username)
      };

      if (user.RoleId == Role.Admin)
        claims.Add(new Claim(ClaimTypes.Role, "Admin"));

      var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
        _configuration.GetSection("AppSettings:Token").Value));

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddMinutes(60),
        signingCredentials: creds);

      var jwt = new JwtSecurityTokenHandler().WriteToken(token);
      return jwt;
    }
  }
}