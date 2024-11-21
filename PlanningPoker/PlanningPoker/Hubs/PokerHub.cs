using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace PlanningPoker.Hubs
{
    public class PokerHub : Hub
    {
        private static readonly ConcurrentDictionary<string, List<string>> RoomUsers = new();
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task JoinRoom(string roomName, string userName)
        {
            var users = RoomUsers.GetOrAdd(roomName, _ => new List<string>());
            lock (users)
            {
                if (!users.Contains(userName))
                {
                    users.Add(userName);
                }
                UserConnections[Context.ConnectionId] = userName;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Caller.SendAsync("ReceiveUserList", users);
            await Clients.OthersInGroup(roomName).SendAsync("UserJoined", $"{userName} has joined the room.");
        }
        public async Task LeaveRoom(string roomName, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{userName} has left the room.");
        }
        public async Task SubmitVote(string roomName, string userName, string vote)
        {
            await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{userName} has voted.");
            await Clients.Group(roomName).SendAsync("ReceiveVote", userName, vote);
        }
        public async Task ClearVotes(string roomName)
        {
            await Clients.Group(roomName).SendAsync("ReceiveMessage", "Votes have been cleared.");
            await Clients.Group(roomName).SendAsync("ClearVotes");
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var (roomName, users) in RoomUsers)
            {
                lock (users)
                {
                    if (UserConnections.TryGetValue(Context.ConnectionId, out var userName))
                    {
                        users.Remove(userName);
                        UserConnections.TryRemove(Context.ConnectionId, out _);

                        // Notify the room about the user leaving
                        Clients.Group(roomName).SendAsync("UserLeft", userName);
                    }
                    if (!users.Any())
                    {
                        RoomUsers.TryRemove(roomName, out _);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
