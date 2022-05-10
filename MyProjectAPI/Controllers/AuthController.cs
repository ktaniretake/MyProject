using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProjectAPI.Models;
namespace MyProjectAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authenticationService, IUserService userService)
    {
      _authService = authenticationService;
      _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserDto request)
    {
      User user;

      try
      {
        user = await _authService.TryRegisterUserAsync(request);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message.ToString());
      }
      
      return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserDto request)
    {
      if (!await _authService.LoginAsync(request, out var token))
        return BadRequest("Wrong Username or Password");

      return Ok(token);
    }

    [HttpGet("get-user-by-id"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
      var user = await _userService.GetUserByIdAsync(id);

      if (user == null)
        return BadRequest("Wrong Id");

      return Ok(user);
    }
  }
}