using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SmartLibrary.Auth.User;

public class UserDbContext : IdentityDbContext<ApplicationUser>
{
  public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
  {
  }

}
