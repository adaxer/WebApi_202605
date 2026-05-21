
using Microsoft.EntityFrameworkCore;
using MovieBase.Common;
using MovieBase.Common.Data;
using System.Diagnostics;

namespace MovieBase.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddScoped<MovieService>();

        builder.Services.AddControllers()
            .AddNewtonsoftJson()
            .AddXmlSerializerFormatters();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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

        app.UseAuthorization();


        app.MapControllers();
        app.MapHub<MessageHub>("/messages");

        app.Run();
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
}
