using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLibrary.Auth.User;
public static class UserDbInitializer
{
  public static async Task EnsureUsers(IServiceProvider services, IEnumerable<(UserLoginData data, IEnumerable<string> roles)> initialUsers)
  {
    using var serviceScope = services.CreateScope();
    var serviceProvider = serviceScope.ServiceProvider;
    var dbContext = serviceProvider.GetRequiredService<UserDbContext>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await dbContext.Database.EnsureCreatedAsync();

    var roleNames = initialUsers.SelectMany(u => u.roles).ToArray();

    foreach (var roleName in roleNames)
    {
      var roleExist = await roleManager.RoleExistsAsync(roleName);
      if (!roleExist)
      {
        await roleManager.CreateAsync(new IdentityRole(roleName));
      }
    }

    foreach (var userTuple in initialUsers)
    {
      try
      {
        var newUser = new ApplicationUser
        {
          UserName = userTuple.data.UserName,
          Email = userTuple.data.Email,
        };
        var createResult = await userManager.CreateAsync(newUser, userTuple.data.Password);
        if (createResult.Succeeded)
        {
          await userManager.AddToRolesAsync(newUser, userTuple.roles);
        }
      }
      catch (Exception ex)
      {
        Trace.TraceError($"Could not initialize {userTuple.data.UserName} / {userTuple.data.Email}: {ex}");
      }
    }

    await dbContext.SaveChangesAsync();
  }
}
