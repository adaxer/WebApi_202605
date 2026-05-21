using Microsoft.AspNetCore.SignalR;

namespace MovieBase.Api;

public class MessageHub : Hub
{
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("Message", message);
    }
}