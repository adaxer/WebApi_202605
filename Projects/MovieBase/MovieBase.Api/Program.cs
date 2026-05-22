
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MovieBase.Common;
using MovieBase.Common.Data;
using SmartLibrary.Auth.Endpoints;
using SmartLibrary.Auth;
using SmartLibrary.Auth.User;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;


namespace MovieBase.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddConfiguredUserDb(o =>
        {
            o.UserDbConnectionString = builder.Configuration.GetConnectionString("AppConnection")!;
            o.EncryptionSecret = builder.Configuration.GetValue<string>("Jwt:Secret")!;
            o.TokenIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer")!;
            o.TokenAudience = builder.Configuration.GetValue<string>("Jwt:Audience")!;
            o.TokenLifetime = builder.Configuration.GetValue<TimeSpan>("Jwt:TokenLifetime")!;
        });

        // Add services to the container.
        builder.Services.AddScoped<MovieService>();

        builder.Services.AddControllers()
            .AddNewtonsoftJson()
            .AddXmlSerializerFormatters();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks()
            .AddCheck<DatabaseConnectedHealthCheck>("Database");

        builder.Services.AddDbContext<MoviesContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("MovieConnection")));

        builder.Services.AddSignalR(c => c.KeepAliveInterval = TimeSpan.FromSeconds(15));

        builder.Services.AddHostedService<MessageService>();

        var app = builder.Build();

        // Seed Database with movies
        SeedDemoData(app.Services);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            // Nur devmode
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapAccountEndpoints();
        app.MapHub<MessageHub>("/messages");
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = WriteHealthCheckResponse
        });

        await UserDbInitializer.EnsureUsers(app.Services, GetUserData());
        app.Run();
    }

    private static Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Headers["X-Health-Writer"] = "custom";

        var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()
                      ?? "unknown";

        var response = new
        {
            status = report.Status.ToString(),
            version,
            checks = report.Entries.Select(e => new {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.ToString(),
                exception = e.Value.Exception?.Message,
                version=version
            }),
            totalDuration = report.TotalDuration.ToString()
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
 

    private static void SeedDemoData(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MoviesContext>();
        try
        {
            var watch = Stopwatch.StartNew();
            db.SeedData();
            Trace.TraceInformation($"Database seed took: {watch.Elapsed}");
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Seeding failed: {ex}");
        }
    }

    private static IEnumerable<(UserLoginData data, IEnumerable<string> roles)> GetUserData()
    {
        yield return (new UserLoginData
        {
            UserName = "alice",
            Email = "alice@bob.com",
            Password = "123Admin!"
        }, new List<string> { "Admin", "User" });

        yield return (new UserLoginData
        {
            UserName = "bob",
            Email = "bob@alice.com",
            Password = "123User!"
        }, new List<string> { "User" });
    }

}
