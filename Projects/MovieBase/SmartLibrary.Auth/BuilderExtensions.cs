using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmartLibrary.Auth.User;
using System.Security.Claims;
using System.Text;

namespace SmartLibrary.Auth;

public static class BuilderExtensions
{
    public static IServiceCollection AddConfiguredUserDb(this IServiceCollection services, Action<ConfigureUserDbOptions> configure = default!)
    {
        var options = new ConfigureUserDbOptions();
        if (configure != default)
        {
            configure(options);
        }

        services.AddSingleton(options);

        services.AddDbContext<UserDbContext>(o =>
            o.UseSqlite(options.UserDbConnectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<UserDbContext>();

        var key = Encoding.ASCII.GetBytes(options.EncryptionSecret);
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "admin"));
            options.AddPolicy("BasicAuth", policy =>
            {
                policy.RequireAuthenticatedUser();
            });
        });

        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
