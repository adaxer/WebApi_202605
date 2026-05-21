using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;

namespace MovieBase.ClientLib;

public class MessageClient
{
    private bool isInitialized = false;
    private HubConnection? connection;
    const string HostUrl = "https://localhost:7184/messages";

    public event Action<string> MessageReceived = s => { };

    public async Task<bool> Initialize()
    {
        if (isInitialized)
        {
            return true;
        }

        try
        {
            HubConnectionBuilder builder = new HubConnectionBuilder();
            connection = builder.WithUrl(HostUrl).WithStatefulReconnect().Build();
            connection.On<string>("Message", m=> MessageReceived.Invoke(m));
            await connection.StartAsync();
            isInitialized = true;
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Connection failed: {ex}");
        }

        return isInitialized;
    }

    public async void SendMessage(string message)
    {
        if(!isInitialized)
        {
            return;
        }

        await connection!.SendAsync("SendMessage", message);
    }
}
