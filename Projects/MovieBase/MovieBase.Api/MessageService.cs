

using Microsoft.AspNetCore.SignalR;
using MovieBase.Common;
using System.Diagnostics;

namespace MovieBase.Api;

internal class MessageService : IHostedService
{
    private readonly IServiceProvider serviceProvider;

    public MessageService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        StartMessaging();
        return Task.CompletedTask;
    }

    private async void StartMessaging()
    {
        await Task.Delay(3000);
        while (true)
        {
            await SendMessage();
            await Task.Delay(10000);
        }
    }

    private async Task SendMessage()
    {
        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<MovieService>();
        var movies = await service.GetMovies();
        var movie = movies.Skip(new Random().Next(1, movies.Count)).FirstOrDefault();
        if (movie == null)
        {
            return;
        }
        var hub = scope.ServiceProvider.GetRequiredService<IHubContext<MessageHub>>();
        await hub.Clients.All.SendAsync("Message", $"Neues Movie: {movie.Title}!!");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}