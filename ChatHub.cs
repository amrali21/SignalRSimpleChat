using Microsoft.AspNetCore.SignalR;
namespace SignalRSimpleChat;

public class ChatHub () : Hub
{
    public const string HubUrl = "/chathub";
    public const string SendToAllClient = "SendToAllClient";
    public static Dictionary<string, string> ConnectedUsers = new Dictionary<string, string>();

    public static event EventHandler? UserJoinLeave;
    public static void OnUserJoinLeave(EventArgs e)
    {
        UserJoinLeave?.Invoke(null, e);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? e)
    {
        RemoveUser(Context.ConnectionId);
        OnUserJoinLeave(new EventArgs());

        await base.OnDisconnectedAsync(e);
    }

    public async Task SendToAll(string from, string message)
    {
        await Clients.All.SendAsync( SendToAllClient, from, message);
    }

    public static void RemoveUser(string id)
    {
        ConnectedUsers.Remove(id);
        OnUserJoinLeave(null);
    }

    public static void AddUser(string id, string name)
    {
        if (!ConnectedUsers.ContainsKey(id))
        {
            ConnectedUsers.Add(id, name);
            OnUserJoinLeave(null);
        }
    }
}