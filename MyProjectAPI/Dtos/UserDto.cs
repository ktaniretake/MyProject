namespace MyProjectAPI
{
  public class UserDto
  {
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FirstName { get; set; } 
    public string? LastName { get; set; } 
    public bool IsAdmin { get; set; }
    public string? AdminsSecretKey { get; set; }
  }
}