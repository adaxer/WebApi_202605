using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SmartLibrary.Auth.User;

namespace SmartLibrary.Auth.Endpoints;

public static class AccountEndpoints
{

    public static void MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", RegisterAsync);
        app.MapPost("/auth/login", LoginAsync);
        app.MapGet("/auth/userinfo", UserInfoAsync).RequireAuthorization();
    }

    private static async Task<IResult> RegisterAsync(UserLoginData data, IUserService userService)
    {
        var success = await userService.TryRegister(data, "User");
        return success
          ? Results.Ok()
          : Results.BadRequest("Can't register");
    }

    private static async Task<IResult> LoginAsync(UserLoginData data, IUserService userService)
    {
        if (await userService.TryLogin(data) is { success: true } loginResult)
        {
            return Results.Ok(new { AccessToken = loginResult.token });
        }
        else
        {
            return Results.BadRequest("Can't login");
        }
    }

    private static async Task<IResult> UserInfoAsync(HttpContext context, UserManager<ApplicationUser> userManager)
    {
        var user = (await userManager.GetUserAsync(context.User))!;
        var roles = (await userManager.GetRolesAsync(user))!;
        return Results.Ok(new UserInfo { Email = user.Email, UserName = user.UserName, Roles = roles.ToList() });
    }
}
