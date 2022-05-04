using System.ComponentModel.DataAnnotations;

namespace MyProjectAPI.Models
{
  public enum Role
  {
    Admin,
    User
  }
  public class User
  {
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public Role RoleId { get; set; }
    public DateTime RegistrationDate { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
  }
}