using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SignalRSimpleChat;

public class ChatHub (AuthenticationStateProvider AuthenticationStateProvider) : Hub
{
    public const string HubUrl = "/chathub";
    public const string SendToAllClient = "SendToAllClient";
    public static readonly Dictionary<string, string> ConnectedUsers = new Dictionary<string, string>();

    private readonly AuthenticationStateProvider _authenticationStateProvider = AuthenticationStateProvider;

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? e)
    {
        RemoveUser(Context.ConnectionId);
        await base.OnDisconnectedAsync(e);
    }


    public async Task SendToAll(string from, string message)
    {
        await Clients.All.SendAsync( SendToAllClient, from, message);
    }

    public static void RemoveUser(string id) =>
        ConnectedUsers.Remove(id);

    public static void AddUser(string id, string name)
    {
        if (!ConnectedUsers.ContainsKey(id))
            ConnectedUsers.Add(id, name);
    }

}