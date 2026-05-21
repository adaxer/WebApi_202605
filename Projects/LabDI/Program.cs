using LabDI;
using LabDI.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddTransient<IOperationTransient, OperationService>();
builder.Services.AddSingleton<IOperationSingleton, OperationService>();
builder.Services.AddScoped<IOperationScoped, OperationService>();
builder.Services.AddSerilog();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/singleton", ([FromServices] IOperationSingleton operation) =>
{
    return operation.OperationId;
});

app.MapGet("/scoped", (IOperationScoped operation, IServiceProvider provider) =>
{
    return $"Direkt: {operation.OperationId}, Über Transient: {provider.GetRequiredService<IOperationTransient>().OperationId}, Über Singleton: {provider.GetRequiredService<IOperationSingleton>().OperationId}, Über Scoped: {provider.GetRequiredService<IOperationScoped>().OperationId}";
});

app.MapGet("/transient", (IOperationTransient operation) =>
{
    return operation.OperationId;
});

app.MapGet("/all", (IOperationTransient transient, IOperationScoped scoped, IOperationSingleton singleton, ILogger<OperationService> logger) =>
{
    logger.LogInformation("Transient: {Transient}, Scoped: {Scoped}, Singleton: {Singleton}", transient.OperationId, scoped.OperationId, singleton.OperationId);
    return new
    {
        Transient = transient.OperationId,
        Scoped = scoped.OperationId,
        Singleton = singleton.OperationId
    };
});

app.Run();
