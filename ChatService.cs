using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using System.Security.Claims;

namespace SignalRSimpleChat;

public class ChatService( ILogger<ChatService> logger, AuthenticationStateProvider AuthenticationStateProvider) : IAsyncDisposable
{
    private HubConnection _connection;
    private readonly ILogger _logger = logger;

    private readonly AuthenticationStateProvider _authenticationStateProvider = AuthenticationStateProvider;
    private ClaimsPrincipal User { get; set; }

    public async Task ConnectAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        User = authState.User;

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
            ChatHub.AddUser(_connection.ConnectionId, User.Identity.Name);
        }
    }

    public Action<string, string> OnSendToAllMessage { get; set; }

    public async Task SendToAll(string sender, string message)
    {
        var serverMethodName = nameof( ChatHub.SendToAll );
        await _connection.InvokeAsync( serverMethodName, sender, message );
    }

    public async ValueTask DisposeAsync()
    {
        if ( _connection != null )
        {
            await _connection.DisposeAsync( );
        }
    }
}