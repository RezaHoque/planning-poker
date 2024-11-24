using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Services;
using System.Collections.Concurrent;

namespace PlanningPoker.Hubs
{
    public class PokerHub : Hub
    {
        private static readonly ConcurrentDictionary<string, List<string>> RoomUsers = new();
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();
        private static readonly ConcurrentDictionary<string, string> UserAvatars = new();
        private static readonly ConcurrentDictionary<string, string> UserVotes = new();
        private readonly IavatarService _avatarService;

        public PokerHub(IavatarService avatarService)
        {
            _avatarService = avatarService;
        }
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

                if (!UserAvatars.ContainsKey(userName))
                {
                    UserAvatars[userName] = _avatarService.GetAvatar(userName, roomName).Result;
                }

                UserConnections[Context.ConnectionId] = userName;
            }
            var userListWithAvatars = users.Select(user => new
            {
                UserName = user,
                Avatar = UserAvatars[user]
            }).ToList();

            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            // await Clients.Group(roomName).SendAsync("UserJoined", userName, users);
            await Clients.Group(roomName).SendAsync("UserJoined", new
            {
                UserName = userName,
                Avatar = UserAvatars[userName]
            }, userListWithAvatars);

            await Clients.Caller.SendAsync("ReceiveUserList", userListWithAvatars);
            //await Clients.OthersInGroup(roomName).SendAsync("UserJoined", $"{userName} has joined the room.");
        }

        public async Task LeaveRoom(string roomName, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{userName} has left the room.");
        }
        public async Task SubmitVote(string roomName, string userName, string vote)
        {
            await Clients.Group(roomName).SendAsync("ReceiveVote", userName, vote);
        }
        public async Task RevealVotes(string roomName, string[] userVoted, string[] votes)
        {
            List<int> intVotes = new List<int>();
            foreach (string str in votes)
            {
                if (int.TryParse(str, out int val))
                {
                    intVotes.Add(val);
                }
            }
            var avarageVotes = intVotes.Sum() / userVoted.Count();

            await Clients.Group(roomName).SendAsync("ReceiveAvarageVote", avarageVotes);
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
