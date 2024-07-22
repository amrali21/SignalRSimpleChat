using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRSimpleChat;

public class ChatService( ILogger<ChatService> logger ) : IAsyncDisposable
{
    private HubConnection _connection;

    private readonly ILogger _logger = logger;

    public async Task ConnectAsync()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl( $"https://localhost:44393{ChatHub.HubUrl}" )
            .Build();

        _connection.On<string, string>( ChatHub.SendToAllClient, (user, message) =>
        {
            OnSendToAllMessage.Invoke( user, message );
        } );

        if ( _connection.State != HubConnectionState.Connected )
        {
            await _connection.StartAsync( );
        }
    }

    public Action<string, string> OnSendToAllMessage { get; set; }

    public async Task SendToAll(string sender, string message)
    {
        var serverMethodName = nameof( ChatHub.SendToAll );
        await _connection.InvokeAsync( serverMethodName, sender, message );
    }

    public void AddUser(string id, string name)
    {
        if (ChatHub.ConnectedUsers.ContainsValue(name))
            ChatHub.ConnectedUsers.Add(id, name);
    }

    public void RemoveUser(string id)
    {
        ChatHub.ConnectedUsers.Remove(id);
    }

    public async ValueTask DisposeAsync()
    {
        if ( _connection != null )
        {
            await _connection.DisposeAsync( );
        }
    }
}