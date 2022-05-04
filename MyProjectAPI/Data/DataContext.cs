using Microsoft.EntityFrameworkCore;
using MyProjectAPI.Models;

namespace MyProjectAPI.Data
{
  public class DataContext : DbContext
  {
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
  }
}